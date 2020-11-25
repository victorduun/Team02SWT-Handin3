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
        public void Ready_DoorOpen_DisplayDisplayLightOn()
        {
            //State == Ready as default
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
             str.Contains("Light is turned on")));
        }

        [Test]
        public void DoorOpen_DoorClosed_DisplayLightOff()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty); 
            //Now in DoorOpen. Light is on
            _door.Closed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
             str.Contains("Light is turned off")));
        }

        [Test]
        public void DoorOpen_PowerButtonPressed_LightAlreadyOn()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);
            //Now in DoorOpen. Light is on
            _output.ClearReceivedCalls(); //Clear output calls 
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Light is already on and display shouldn't show "Light is turned on"
            _output.DidNotReceive().OutputLine(Arg.Is<string>(str =>
            str.Contains("Light is turned on")));
        }

        [Test]
        public void DoorOpen_SetTimeButtonPressed_LightAlreadyOn()
        {
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);
            //Now in DoorOpen. Light is on
            _output.ClearReceivedCalls(); //Clear output calls 
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Light is already on and display shouldn't show "Light is turned on"
            _output.DidNotReceive().OutputLine(Arg.Is<string>(str =>
            str.Contains("Light is turned on")));
        }

        [Test]
        public void SetTimePressed_StartCancelPressed_DisplayLightOn()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
           
            _output.Received(1).OutputLine(Arg.Is<string>(str =>
            str.Contains("Light is turned on")));
        }

        [Test]
        public void Cooking_StartCancelPressed_DisplayLightOff()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in Cooking
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
            str.Contains("Light is turned off")));
        }

        [Test]
        public void Cooking_CookingIsDone_DisplayLightOff()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in Cooking
            _uut.CookingIsDone();

            _output.Received(1).OutputLine(Arg.Is<string>(str =>
            str.Contains("Light is turned off")));
        }

        [Test]
        public void Ready_PowerPressed_DisplayOutput50()
        {
            //State == Ready as default
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("50")));
        }

        [Test]
        public void Ready_2PowerPressed_DisplayOutput100()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("100")));
        }

        [Test]
        public void SetPower_TimePressed_DisplayOutput1Colon00()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("1:00")));
        }

        [Test]
        public void SetPower_2TimePressed_DisplayOutput2Colon00()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("2:00")));
        }

        [Test]
        public void SetPower_StartCancelPressed_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("Display cleared")));
        }

        [Test]
        public void Cooking_StartCancelPressed_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in Cooking
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("Display cleared")));
            
        }

        [Test]
        public void PowerPressed_DoorOpen_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);
            
            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("Display cleared")));
        }

        [Test]
        public void TimePressed_DoorOpen_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime 
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("Display cleared")));
        }

        [Test]
        public void Cooking_DoorOpen_DisplayCleared()
        {
            _powerButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetPower
            _timeButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in SetTime
            _startCancelButton.Pressed += Raise.EventWith(this, EventArgs.Empty);
            //Now in Cooking
            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            _output.Received(1).OutputLine(Arg.Is<string>(str => 
            str.Contains("Display cleared")));
        }
    }
}