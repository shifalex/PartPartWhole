using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Sentry;

namespace PartPartWhole.ViewModels
{
    public partial class PartPartWholeViewModel : ObservableObject
    {
        private readonly int NAN = -1111;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(SAddent1))]
        [AlsoNotifyChangeFor(nameof(Addent1Enabled))]
        private int _addent1;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(SAddent2))]
        [AlsoNotifyChangeFor(nameof(Addent2Enabled))]
        private int _addent2;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(SSum))]
        [AlsoNotifyChangeFor(nameof(SumEnabled))]
        private int _sum;

        public bool SumEnabled { get { return (_sum == NAN); } }
        public bool Addent1Enabled { get { return (_addent1 == NAN); } }
        public bool Addent2Enabled { get {  return(_addent2 == NAN);  } }

        public String SAddent1
        {
            get
            {
                if (_addent1 == NAN) return "";
                return _addent1.ToString();
            }
            set
            {
                try { _addent1 = Int32.Parse(value); } catch { _addent1 = NAN; }
                OnPropertyChanged(nameof(Addent1)); OnPropertyChanged(nameof(TrueStatement));
            }
        }
        public String SAddent2
        {
            get
            {
                if (_addent2 == NAN) return "";
                return _addent2.ToString();
            }
            set
            {
                try { _addent2 = Int32.Parse(value); } catch { _addent2 = NAN; }
                OnPropertyChanged(nameof(Addent2)); OnPropertyChanged(nameof(TrueStatement));
            }
        }
        public String SSum
        {
            get
            {
                if (_sum == NAN) return "";
                return _sum.ToString();
            }
            set
            {
                try { _sum = Int32.Parse(value); } catch { _sum = NAN; }
                OnPropertyChanged(nameof(Sum)); OnPropertyChanged(nameof(TrueStatement));
            }
        }



        //[ObservableProperty]
        //private List<PPWObject> _allHistory = new();


        //SETTINGS!
        [ObservableProperty]
        private int _fMustFindOneTwoBoth = 2; //TODO: make a nice ENUM
        [ObservableProperty]
        private bool _fMustFindTheSum = true;
        [ObservableProperty]
        private int _maxAddent = 5;
        [ObservableProperty]
        private int _minAddent = 0;
        [ObservableProperty]
        private int _maxSum = 10;
        [ObservableProperty]
        private bool _fInsisitentOnOne = false;
        private int _lastNum = 10;

        [ObservableProperty]
        private int _streakCorrect = 0;
        [ObservableProperty]
        private int _streakWrong = 0;

        [ObservableProperty]
        private int _decompositionLevel = -1;
        private int _oldlevel = 2;
        public bool IsDecomposition
        {
            get { return _decompositionLevel != -1; }
            set
            {
                if (!value)
                {
                    _oldlevel = _decompositionLevel;
                    DecompositionLevel = -1;
                }
                else
                {
                    if (_decompositionLevel == -1 || _decompositionLevel == 4)
                        DecompositionLevel = _oldlevel;
                    FMustFindOneTwoBoth = 1;
                    FMustFindTheSum = false;
                    MaxAddent = 20;
                    MinAddent = 1;
                    MaxSum = 20;
                    FInsisitentOnOne = false;

                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotDecomposition));
            }
        }
        public bool IsNotDecomposition { get { return !IsDecomposition; } }

        //TODO:make a nice Enum
        public String TrueStatement
        {
            get
            {
                SentrySdk.CaptureMessage("Getting true statement");
                if (_addent1 == NAN || _addent2 == NAN || _sum == NAN) return "";
                else if (_addent1 > _maxAddent || _addent1 < _minAddent || _addent2 > _maxAddent || _addent2 < _minAddent || _sum > _maxSum || _sum < _minAddent) return "wrong input!";
                else if (_sum == _addent1 + _addent2)
                {
                    OnPropertyChanged(nameof(SumEnabled)); OnPropertyChanged(nameof(Addent1Enabled)); OnPropertyChanged(nameof(Addent2Enabled));
                    /*_allHistory.Add(new(_addent1, _addent2, _sum));*/
                                        
                    if (_decompositionLevel > 0)
                    {
                        StreakCorrect++; StreakWrong = 0;
                        if(StreakCorrect >= 20)
                        {
                            DecompositionLevel++; StreakCorrect = 0;
                            if (_decompositionLevel > 3)
                            {

                                SentrySdk.CaptureMessage("Win");
                                App.Current.MainPage.DisplayAlert("Win", "You Won!!", "OK");
                                return "YOU WON!!!!!!";
                            }
                        }
                    }
                    SentrySdk.CaptureMessage("Correct");
                    return "CORRECT :D";
                }
                else
                {

                    if (_decompositionLevel > 0)
                    {
                        StreakWrong++;
                        if (StreakWrong > 5)
                        {
                            DecompositionLevel--; StreakCorrect = 0; StreakWrong = 0;
                            if (_decompositionLevel == 0)
                            {
                                SentrySdk.CaptureMessage("Lose");

                                App.Current.MainPage.DisplayAlert("Lose", "You Lost!!", "OK");
                                return "YOU LOST!!!!!!";
                            }
                        }
                    }
                    SentrySdk.CaptureMessage("Incorrect");
                    return "WRONG :(";
                }
            }
        }


        [ICommand]
        public void Default()
        {
             DecompositionLevel = -1;
             FMustFindOneTwoBoth = 2; //TODO: make a nice ENUM
             FMustFindTheSum = true;       
             MaxAddent = 5;
             MinAddent = 0;
             MaxSum = 10;
             FInsisitentOnOne = false;
        }

        [ICommand]
        public void GenerateExercise()
        {
            SentrySdk.CaptureMessage("Hello Sentry");
            if (_decompositionLevel == 1) { MaxAddent = 10; MaxSum = 10; FInsisitentOnOne = true; }
            if (_decompositionLevel == 2) { MaxAddent = 20; MaxSum = 20; FInsisitentOnOne = false; }
            if (_decompositionLevel == 3) { MaxAddent = 100; MaxSum = 100; }
            //TODO: validation also in the form with Binding
            if (_maxSum > 2 * _maxAddent) _maxSum = 2 * _maxAddent;

           
            int[] factors = new int[3];
            Random r = new();
            factors[2] = r.Next(2 * _minAddent, _maxSum + 1);
            if (_fInsisitentOnOne) factors[2] = _lastNum;
            factors[0] = r.Next(_minAddent, Math.Min(_maxAddent, factors[2])+1);
            factors[1] = factors[2] - factors[0];

            SentrySdk.CaptureMessage("First factors success");

            
            
            if (_decompositionLevel > 1)
            {
                if (_sum != _addent1 + _addent2) StreakWrong++;
                factors[2] = r.Next(Math.Max(_minAddent, 10), _maxSum);
                while (factors[2]%10==9) factors[2] = r.Next(Math.Max(_minAddent, 10), _maxSum);
                if (factors[2] % 10 == 0) factors[0]= r.Next(_minAddent, Math.Min(_maxAddent+ 1, factors[2]) );
                else { 
                    int tens = r.Next(Math.Max(_minAddent / 10, 0), factors[2] / 10);
                    int ones = r.Next(factors[2] % 10 + 1, 10);
                    factors[0] = tens * 10 + ones;
                }
                factors[1] = factors[2] - factors[0];
            }
            SentrySdk.CaptureMessage("Second factors success");


            int questionType;
            if (_fMustFindOneTwoBoth == 1) questionType = 1;
            else if (_fMustFindOneTwoBoth == 2) questionType = 2;
            else questionType = r.Next(2);
            int n = r.Next(3);
            if (_fMustFindTheSum) n = 2;
            if (questionType == 1)
                factors[n] = NAN;
            else
                for (int i = 0; i < 3; i++)
                    if (i != n) factors[i] = NAN;
            SentrySdk.CaptureMessage("Xs success");

            Addent1 = factors[0];
            Addent2 = factors[1];
            Sum = factors[2];


            SentrySdk.CaptureMessage("Pulling the entries success");

        }

    }
}