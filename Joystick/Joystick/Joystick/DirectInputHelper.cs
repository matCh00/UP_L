using System;
using System.Collections.Generic;
using System.Text;
using SharpDX.DirectInput;
using System.Threading;

namespace Joystick
{
    class DirectInputHelper
    {

        DirectInput directInput;
        DeviceInstance gamepadDevice;
        public int valueX;
        public int valueY;
        public int valueTriggers;
        public bool isButtonClicked;

        public DirectInputHelper()
        {
            valueX = 65535 / 2;
            valueY = 65535 / 2;
            valueTriggers = 0;
            isButtonClicked = false;
            directInput = new DirectInput();
            gamepadDevice = null;
        }

        public string getGamepadName()
        {
            if (gamepadDevice == null)
                return "Brak podlaczonego gamepada";

            return gamepadDevice.InstanceName;
        }

        public void connectGamepad()
        {
            gamepadDevice = null;
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                gamepadDevice = deviceInstance;
                Console.WriteLine(deviceInstance.InstanceName);
            }

            Thread thread = new Thread(readInput);
            thread.Start();
        }

        public void readInput()
        {

            if (gamepadDevice == null)
                return;

            SharpDX.DirectInput.Joystick joystick = new SharpDX.DirectInput.Joystick(directInput, gamepadDevice.InstanceGuid);
            joystick.Properties.BufferSize = 128;

            joystick.Acquire();
            while (true)
            {
                if (joystick.IsDisposed)
                    return;
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                {
                    if (state.Offset.ToString().Equals("X"))
                    {
                        valueX = state.Value;
                        Console.WriteLine(valueX);
                    }
                    else if (state.Offset.ToString().Equals("Y"))
                    {
                        valueY = state.Value;
                    }
                    else if (state.Offset.ToString().Equals("Buttons0"))
                    {
                        switch (state.Value)
                        {
                            case 0: isButtonClicked = false; break;
                            case 128: isButtonClicked = true; break;
                        }
                    }
                    else if (state.Offset.ToString().Equals("Z"))
                    {
                        valueTriggers = state.Value;
                    }
                }
            }
        }
    }
}
