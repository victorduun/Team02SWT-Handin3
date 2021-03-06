using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NUnit.Framework;
using NSubstitute;

namespace Microwave.Test.Integration
{
    /* Cook Controller, Display, Timer, Powertube */
    public class IT1
    {
        private CookController _uut;

        private IOutput _fakeOutput;
        private IDisplay _display;
        private ITimer _timer;
        private IPowerTube _powerTube;
        private IUserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _userInterface = Substitute.For<IUserInterface>();
            _fakeOutput = Substitute.For<IOutput>();
            _display = new Display(_fakeOutput);
            _powerTube = new PowerTube(_fakeOutput);
            _timer = new Timer();
            _uut = new CookController(_timer, _display, _powerTube, _userInterface);
        }

        [Test]
        public void StartCooking_UserInterfaceCheckIfDoneAfterTimerExpired_NotDone()
        {
            _uut.StartCooking(100, 1);
            
            _userInterface.Received(0).CookingIsDone();
        }

        [Test]
        public void StartCooking_UserInterfaceCheckIfDoneAfterTimerExpired_Done()
        {
            _uut.StartCooking(100, 1);
            System.Threading.Thread.Sleep(2000);
            _userInterface.Received(1).CookingIsDone();
        }

        [Test]
        public void StartCooking_PowerTubeCheckIfDoneAfterTimerExpired_Done()
        {
            _uut.StartCooking(100, 1);
            System.Threading.Thread.Sleep(2000);
            _fakeOutput.Received(2).OutputLine(Arg.Is<string>(str=>
                str.ToLower().Contains("powertube turned off") || 
                str.ToLower().Contains("powertube works with")
                ));
        }

        [Test]
        public void StartCooking_StartAndStopCooking_NoCallBackToUserInterface()
        {
            _uut.StartCooking(100, 1);
            _uut.Stop();
            System.Threading.Thread.Sleep(2000);

            _userInterface.Received(0).CookingIsDone();
        }

        [Test]
        public void StartCooking_StartAndStopCooking_NoOutput()
        {
            _uut.StartCooking(100, 1);
            _fakeOutput.ClearReceivedCalls();
            _uut.Stop();
            System.Threading.Thread.Sleep(2000);

            _fakeOutput.Received(1).OutputLine(Arg.Is<string>(str => 
                str.ToLower().Contains("powertube turned off")));
        }

        [Test]
        public void StartCooking_10SecondTimer_WritesToConsole10Times()
        {
            _uut.StartCooking(100, 10);
            _fakeOutput.ClearReceivedCalls();
            System.Threading.Thread.Sleep(15000);

            _fakeOutput.Received(10).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display shows: ")));
        }


        [TestCase(5, 30)]
        [TestCase(3, 30)]
        [TestCase(1, 30)]
        [TestCase(0, 30)]
        [TestCase(0, 4)]
        public void StartCooking_DisplayCorrectTime_TimeIsCorrect(int minutes, int seconds)
        {
            int time = minutes * 60 + seconds;
            _uut.StartCooking(100, time);
            
            System.Threading.Thread.Sleep(1100);
            int secondsToCheck = seconds - 1;
            _fakeOutput.Received(1).OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains($"display shows: {minutes:D2}:{secondsToCheck:D2}")));
        }
    }
}