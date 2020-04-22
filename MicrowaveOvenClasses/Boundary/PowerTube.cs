using System;
using MicrowaveOvenClasses.Interfaces;

namespace MicrowaveOvenClasses.Boundary
{
    public class PowerTube : IPowerTube
    {
        private IOutput myOutput;

        private bool IsOn = false;

        public PowerTube(IOutput output)
        {
            myOutput = output;
        }

        //Lavet en ændring så mikrobølgeoven nu tager imellem 50-700W istedet for 1-100%.
        public void TurnOn(int power)
        {
            if (power < 50 || 700 < power)
            {
                throw new ArgumentOutOfRangeException("power", power, "Must be between 50W and 700W (incl.)");
            }

            if (IsOn)
            {
                throw new ApplicationException("PowerTube.TurnOn: is already on");
            }

            myOutput.OutputLine($"PowerTube works with {power}");
            IsOn = true;
        }

        public void TurnOff()
        {
            if (IsOn)
            {
                myOutput.OutputLine($"PowerTube turned off");
            }

            IsOn = false;
        }
    }
}