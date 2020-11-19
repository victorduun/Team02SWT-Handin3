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
    /* User interface, Light, Display*/
    [TestFixture]
    public class IT2
    {
        //private StringWriter str;

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
        public void Ready_DoorOpen_LightOn()
        {
            // This test that uut has subscribed to door opened, and works correctly
            // simulating the event through NSubstitute

            _door.Opened += Raise.EventWith(this, EventArgs.Empty);

            //Assert.That(str.ToString().Contains("Light is turned on"));
        }


    }
}