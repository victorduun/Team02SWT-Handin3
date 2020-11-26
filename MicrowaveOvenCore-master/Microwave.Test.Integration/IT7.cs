using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT7
    {
        private IDoor _uut;
        private IButton _timeButton;
        private IButton _powerButton;
        private IButton _startCancelButton;
        private IOutput _output;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private ILight _light;
        private ITimer _timer;
        private CookController _cooker;
        private UserInterface _userInterface;
        
        [SetUp]
        public void Setup()
        {
            _uut = new Door();
            _timeButton = new Button();
            _powerButton = new Button();
            _startCancelButton = new Button();
            _output = new Output();
            _display = new Display(_output);
            _powerTube = new PowerTube(_output);
            _light = new Light(_output);
            _timer = new Timer();
            _cooker = new CookController(_timer, _display, _powerTube);
            _userInterface = new UserInterface(_powerButton, _timeButton, _startCancelButton, _uut, _display, _light, _cooker);
            _cooker.UI = _userInterface;
        }

        [Test]
        public void Ready_OpenDoor_()
        {
            _uut.Opened += Raise.EventWith<EventArgs>();
            _userInterface.Received(1).OnDoorOpened(this, EventArgs.Empty);
 
        }



    }
}
