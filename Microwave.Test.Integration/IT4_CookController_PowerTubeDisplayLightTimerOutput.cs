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
        public void CookController_TurnOnThrowsExceptionIfPowerIsOutOfRangePowerTube(int power)
        {
            Assert.That(() => _cookController.StartCooking(power,60), Throws.TypeOf<ArgumentOutOfRangeException>());
        }


        [TestCase(50)]
        [TestCase(150)]
        [TestCase(700)]
        public void CookController_TurnOnThrowsExceptionIfPowerIsAlreadyOnPowerTube(int power)
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



        [Test]
        public void CookController_GetTimeRemainingTimer()
        {
            _cookController.StartCooking(50, 60);
            System.Threading.Thread.Sleep(1050);
            _output.Received().OutputLine(Arg.Is<string>(s => s.Contains("Display shows: 00:59")));
        }


        //Vi ved godt dette er noget fis, men gør det for at drille Lasse
        [TestCase(0,10)]
        [TestCase(0,30)]
        [TestCase(1,50)]
        public void CookController_ShowTimeDisplay(int min, int sec)
        {
            _display.ShowTime(min, sec);
            _output.Received().OutputLine(Arg.Is<string>(s => s.Contains($"Display shows: {min:D2}:{sec:D2}")));
        }



        [Test]
        public void CookController_OnTimerExpiredTimer()
        {
            _cookController.StartCooking(50, 1);
            System.Threading.Thread.Sleep(1500);
            _output.Received().OutputLine(Arg.Is<string>(s => s.Contains("PowerTube turned off")));
        }



        [TestCase(50)]
        [TestCase(100)]
        [TestCase(700)]
        public void CookController_TurnOffPowerTube(int power)
        {
            _cookController.StartCooking(power, 60);
            _cookController.Stop();
            _output.Received().OutputLine(Arg.Is<string>(s => s.Contains("PowerTube turned off")));
        }



        //Extentions (Andre extentions er testet tidligere)
        [Test]
        public void CookController_ExtentionStopTimer()
        {
            _cookController.Stop();
            _output.DidNotReceive().OutputLine(Arg.Is<string>(s => s.Contains("PowerTube turned off")));
        }
    }
}
