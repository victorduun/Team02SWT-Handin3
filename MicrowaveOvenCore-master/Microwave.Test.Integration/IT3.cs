using System;
using System.Collections.Generic;
using System.Text;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Microwave.Test.Integration
{
    /*
     * User interface   (T)
     * Cook controller  (x)
     * Light            (x)
     * Display          (x)
     * Timer            (x)
     * PowerTube        (x)
     * Output           (x)
     */
    [TestFixture]
    class IT3
    {
        private IButton _fakepowerButton;
        private IButton _faketimeButton;
        private IButton _fakestartCancelButton;
        private IDoor _fakedoor;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private ILight _light;
        private ITimer _timer;
        private IOutput _fakeOutput;
        private UserInterface _uut;
        private CookController _cooker;
        [SetUp]
        public void SetUp()
        {
            _fakepowerButton = Substitute.For<IButton>();
            _faketimeButton = Substitute.For<IButton>();
            _fakestartCancelButton = Substitute.For<IButton>();
            _fakeOutput = Substitute.For<IOutput>();
            _fakedoor = Substitute.For<IDoor>();
            _display = new Display(_fakeOutput);
            _powerTube = new PowerTube(_fakeOutput);
            _light = new Light(_fakeOutput);
            _timer = new Timer();
            _cooker = new CookController(_timer, _display, _powerTube);
            _uut = new UserInterface(_fakepowerButton, _faketimeButton, _fakestartCancelButton, _fakedoor, _display, _light, _cooker);
        }

        [Test]
        public void OpenDoor_LightOn_LogEqualOn()
        {
            _uut.OnDoorOpened(this,EventArgs.Empty);
            _fakeOutput.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("on")));
        }

        [Test]
        public void CloseDoor_LightOff_LogEqualOff()
        {
            _uut.OnDoorOpened(this, EventArgs.Empty);
            _uut.OnDoorClosed(this, EventArgs.Empty);
            _fakeOutput.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }

        [Test]
        public void PressButton_Power_LogEqual50()
        {
            _uut.OnPowerPressed(this, EventArgs.Empty);
            _fakeOutput.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("50")));
        }

        [Test]
        public void PressButton_Time_LogEqual()
        {
            _uut.OnTimePressed(this, EventArgs.Empty);
            _fakeOutput.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("1:00")));
        }

        [Test]
        public void PressButton_StartCancel_LogEqual()
        {
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _fakeOutput.Received(0).OutputLine(Arg.Is<string>(str=>str.Contains(null)));
        }
    }
}
