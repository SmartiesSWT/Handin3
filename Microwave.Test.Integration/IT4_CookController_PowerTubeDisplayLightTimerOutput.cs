using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;

namespace Microwave.Test.Integration
{
    public class IT4_CookController_PowerTubeDisplayLightTimerOutput
    {
        //Top
        private ICookController _cookController;

        //Stubs
        private IOutput _output;

        //Includes
        private IPowerTube _powerTube;
        private IDisplay _display;
        private ILight _light;
        private ITimer _timer;


        [SetUp]
        public void SetUp()
        {
            //Stubs
            _output = Substitute.For<IOutput>();

            //Includes
            _timer = new Timer();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);

            //Top
            _cookController = new CookController(_timer, _display, _powerTube);
        }


        [TestCase(1)]
        [TestCase(10)]
        [TestCase(60)]
        public void CookController_StartTimer(int time)
        {
                _cookController.StartCooking(50, time);
                Assert.That(_timer.TimeRemaining, Is.EqualTo(time));
        }


        [TestCase(-1)]
        [TestCase(49)]
        [TestCase(701)]
        public void CookController_TurnOnThrowsExeptionIfPowerIsOutOfRangePowerTube(int power)
        {
            Assert.That(() => _cookController.StartCooking(power,60), Throws.TypeOf<ArgumentOutOfRangeException>());
        }


        [TestCase(50)]
        [TestCase(150)]
        [TestCase(700)]
        public void CookController_TurnOnThrowsExeptionIfPowerIsAlreadyOnPowerTube(int power)
        {
            _cookController.StartCooking(power, 60);
            Assert.That(() => _cookController.StartCooking(power, 60), Throws.TypeOf<ApplicationException>());
        }


        [TestCase(50)]
        [TestCase(100)]
        [TestCase(700)]
        public void CookController_TurnOnPowerTube(int power)
        {
            _cookController.StartCooking(power, 60);
            _output.Received().OutputLine(Arg.Is<string>(s => s.Contains($"PowerTube works with {power}")));
        }


        //[TestCase(5)]
        //[TestCase(25)]
        //[TestCase(50)]
        //public void CookController_GetTimeRemainingTimer(int time)
        //{
        //    _timer.TimerTick += Raise.EventWith(new T);
        //}


        //[TestCase(5)]
        //[TestCase(25)]
        //[TestCase(50)]
        //public void CookController_ShowTimeDisplay(int time)
        //{
        //    _cookController.StartCooking(100, time);
        //    Assert.That(_timer.TimeRemaining, Is.EqualTo(time));
        //}


        //[TestCase(5)]
        //[TestCase(25)]
        //[TestCase(50)]
        //public void CookController_CookingIsDoneUserInterface(int time)
        //{
        //    _cookController.StartCooking(100, time);
        //    Assert.That(_timer.TimeRemaining, Is.EqualTo(time));
        //}


        [TestCase(50)]
        [TestCase(100)]
        [TestCase(700)]
        public void CookController_TurnOffPowerTube(int power)
        {
            _cookController.StartCooking(power, 60);
            _cookController.Stop();
            _output.Received().OutputLine(Arg.Is<string>(s => s.Contains("PowerTube turned off")));
        }
    }
}
