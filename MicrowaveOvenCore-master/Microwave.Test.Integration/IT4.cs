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
    public class IT4
    {
        private IButton _fakepowerButton;
        private IButton _faketimeButton;
        private IButton _startCancelButton;
        private IDoor _fakedoor;
        private IDisplay _display;
        private IPowerTube _powerTube;
        private ILight _light;
        private ITimer _fakeTimer;
        private IOutput _fakeOutput;
        private UserInterface _uut;
        private CookController _cooker;
        [SetUp]
        public void Setup()
        {
            _fakepowerButton = Substitute.For<IButton>();
            _faketimeButton = Substitute.For<IButton>();
            _startCancelButton = new Button();
            _fakeOutput = Substitute.For<IOutput>();
            _fakedoor = Substitute.For<IDoor>();
            _display = new Display(_fakeOutput);
            _powerTube = new PowerTube(_fakeOutput);
            _light = new Light(_fakeOutput);
            _fakeTimer = Substitute.For<ITimer>();
            _cooker = new CookController(_fakeTimer, _display, _powerTube);
            _uut = new UserInterface(_fakepowerButton, _faketimeButton, _startCancelButton, _fakedoor, _display, _light, _cooker);
            //Finish double association
            _cooker.UI = _uut;
        }

        [Test]
        public void StartCancelButtonPressed_TimerStartedWithCorrectParameters_TimerStarted()
        {
            _fakepowerButton.Pressed += Raise.EventWith<EventArgs>();
            _faketimeButton.Pressed += Raise.EventWith<EventArgs>();
            _startCancelButton.Press();
            _fakeTimer.Received().Start(60000);
        }

        [Test]
        public void StartCancelButtonPressed_OutputIsCorrect_OutputIsCorrect()
        {
            _fakepowerButton.Pressed += Raise.EventWith<EventArgs>();
            _faketimeButton.Pressed += Raise.EventWith<EventArgs>();
            _fakeOutput.ClearReceivedCalls();
            _startCancelButton.Press();

            _fakeOutput.Received(2).OutputLine(Arg.Any<string>());
        }

    }
}
