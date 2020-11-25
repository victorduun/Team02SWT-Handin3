﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private ITimer _faketimer;
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
            _faketimer = Substitute.For<ITimer>();  //Using fake timer as the integration test would otherwise run for a long time

            _display = new Display(_fakeOutput);
            _powerTube = new PowerTube(_fakeOutput);
            _light = new Light(_fakeOutput);
            _cooker = new CookController(_faketimer, _display, _powerTube);
            _uut = new UserInterface(_fakepowerButton, _faketimeButton, _fakestartCancelButton, _fakedoor, _display, _light, _cooker);
            _cooker.UI = _uut;  // Finish the double association

        }

        /* Initiate Start cooking helper function*/
        public void IT_SetupToCook(int setPower, int setTime)
        {
            _uut.OnDoorOpened(this,EventArgs.Empty);
            _uut.OnDoorClosed(this, EventArgs.Empty);

            for (int i = 0; i < setPower; i++)
            {
                _uut.OnPowerPressed(this, EventArgs.Empty);
            }

            for (int i = 0; i < setTime; i++)
            {
                _uut.OnTimePressed(this, EventArgs.Empty);
            }

        }
     
        [Test]
        public void CookingIsStarted_DisplayShow_100W_ShowTime_1()
        {
            //Set power to 100 and set time to 1min
            IT_SetupToCook(2, 1);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _fakeOutput.Received(3).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on") ||
                str.ToLower().Contains("display shows: 100 W") ||
                str.ToLower().Contains("display shows: 01:00")));
        }
        
        [Test]
        public void CookingIsStarted_DisplayShow_PowerTube_Off_Display_Cleared_Light()
        {
           //Set power to 100 and set time to 1min
            IT_SetupToCook(2, 1);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _fakeOutput.ClearReceivedCalls();
            _faketimer.Expired += Raise.EventWith<EventArgs>();
            _fakeOutput.Received(3).OutputLine(Arg.Is<string>(str => 
                str.ToLower().Contains("powertube turned off") ||
                str.ToLower().Contains("display cleared") ||
                str.ToLower().Contains("light is turned off")));
        }

        [Test]
        public void CookingIsStarted_OnDoorOpen_Display_Cleared_Powertube_Off()
        {
            //Set power to 100 and set time to 1min
            IT_SetupToCook(2, 1);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _fakeOutput.ClearReceivedCalls();
            _uut.OnDoorOpened(this,EventArgs.Empty);
            _fakeOutput.Received(2).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared") ||
                str.ToLower().Contains("powertube turned off")));
        }

        [Test]
        public void CookingIsStarted_Stop_Display_Cleared_Powertube_Off()
        {
            //Set power to 100 and set time to 1min
            IT_SetupToCook(2, 1);
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _fakeOutput.ClearReceivedCalls();
            _uut.OnStartCancelPressed(this, EventArgs.Empty);
            _fakeOutput.Received(3).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared") ||
                str.ToLower().Contains("powertube turned off") ||
                //MyLight.TurnOff() is never called
                str.ToLower().Contains("light is turned off"))); 
        }
    }
}