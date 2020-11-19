using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave.Classes.Boundary;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT2
    {
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDoor _door;
        private ICookController _cooker;
        private IOutput _output;

        private IDisplay _display;
        private ILight _light;

        private UserInterface _uut;

        [SetUp]
        public void Setup()
        {
            //str = new StringWriter();
            //Console.SetOut(str);

            //Stubbed modules 
            _powerButton = Substitute.For<IButton>();
            _timeButton = Substitute.For<IButton>();
            _startCancelButton = Substitute.For<IButton>();
            _door = Substitute.For<IDoor>();
            _output = Substitute.For<IOutput>();
            _cooker = Substitute.For<ICookController>();

            //Included modules 
            _display = new Display(_output);
            _light = new Light(_output);
            
            //Top module
            _uut = new UserInterface(
                _powerButton, _timeButton, _startCancelButton,
                _door,
                _display,
                _light,
                _cooker);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void DoorClosed_DoorOpen_LightOn()
        {
            _door.Closed += Raise.EventWith(this, EventArgs.Empty);
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
             str.Contains("Light is turned on")
             ));
            _light.Received(1).TurnOn();
        }

        [Test]
        public void DoorOpen_DoorClosed_LightOff()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);
            _door.Closed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
             str.Contains("Light is turned off")
            ));
        }

        //Thomas' test // 
        [Test]
        public void OpenDoor_LightOn_LogEqualOn()
        {
            _uut.OnDoorOpened(this, EventArgs.Empty);
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
            _fakeOutput.Received(0).OutputLine(Arg.Is<string>(str => str.Contains(null)));
        }

    }
}