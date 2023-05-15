using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Sentry;

namespace PartPartWhole.ViewModels
{
    public partial class PartPartWholeViewModel : ObservableObject
    {
        private readonly int NAN = -1111;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(TrueStatement))]
        [AlsoNotifyChangeFor(nameof(Addent1Enabled))]
        [AlsoNotifyChangeFor(nameof(Addent2Enabled))]
        [AlsoNotifyChangeFor(nameof(SumEnabled))]
        [AlsoNotifyChangeFor(nameof(SAddent1))]
        private int _addent1;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(TrueStatement))]
        [AlsoNotifyChangeFor(nameof(Addent1Enabled))]
        [AlsoNotifyChangeFor(nameof(Addent2Enabled))]
        [AlsoNotifyChangeFor(nameof(SumEnabled))]
        [AlsoNotifyChangeFor(nameof(SAddent2))]
        private int _addent2;

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(TrueStatement))]
        [AlsoNotifyChangeFor(nameof(Addent1Enabled))]
        [AlsoNotifyChangeFor(nameof(Addent2Enabled))]
        [AlsoNotifyChangeFor(nameof(SumEnabled))]
        [AlsoNotifyChangeFor(nameof(SSum))]
        private int _sum;

        private bool _newFlag1 = true;
        private bool _newFlag2 = true;
        private bool _newFlagSum = true;

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
        public bool IsDecomposition
        {
            get { return _decompositionLevel != -1; }
            set
            {
                if (!value)
                {
                    DecompositionLevel = -1;
                }
                else
                {
                    if (_decompositionLevel == -1 || _decompositionLevel == 4)
                        DecompositionLevel = 2;
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
            factors[0] = r.Next(_minAddent, _maxAddent + 1);
            factors[2] = r.Next(2 * _minAddent, _maxSum + 1);
            while (factors[2] < factors[0] || (factors[2] - factors[0]) > _maxAddent || (factors[2] - factors[0]) < _minAddent)
                factors[2] = r.Next(0, _maxSum + 1);

            //TODO: FIX make an entry near checkbox which will be enabled only when checked
            if (_fInsisitentOnOne)
            {
                factors[2] = _lastNum;
                while (factors[2] < factors[0] || (factors[2] - factors[0]) > _maxAddent || (factors[2] - factors[0]) < _minAddent)
                    factors[0] = r.Next(_minAddent, _maxAddent + 1);
            }
            SentrySdk.CaptureMessage("First factors success");

            factors[1] = (factors[2] - factors[0]);
            if (_decompositionLevel > 0)
            {
                while ((factors[2] % 10 > factors[0] % 10) || factors[2] < factors[0] || factors[1] > _maxAddent || factors[1] < _minAddent)
                {
                    factors[0] = r.Next(_minAddent, _maxAddent + 1);
                    factors[2] = r.Next(2 * _minAddent, _maxSum + 1);
                    factors[1] = (factors[2] - factors[0]);
                }
            }

            SentrySdk.CaptureMessage("Second factors success");


            int QuestionType;
            if (_fMustFindOneTwoBoth == 1) QuestionType = 1;
            else if (_fMustFindOneTwoBoth == 2) QuestionType = 2;
            else QuestionType = r.Next(2);
            int n = r.Next(3);
            if (_fMustFindTheSum) n = 2;
            if (QuestionType == 1)
                factors[n] = NAN;
            else
                for (int i = 0; i < 3; i++)
                    if (i != n) factors[i] = NAN;

            SentrySdk.CaptureMessage("Xs success");

            _newFlag1 = (factors[0] == NAN);
            _newFlag2 = (factors[1] == NAN);
            _newFlagSum = (factors[2] == NAN);

            Addent1 = factors[0];
            Addent2 = factors[1];
            Sum = factors[2];


            SentrySdk.CaptureMessage("Pulling the entries success");

        }



        //TODO:make a nice Enum
        public String TrueStatement
        {
            get
            {

                SentrySdk.CaptureMessage("Getting true statement");
                if (_addent1 == NAN || _addent2 == NAN || _sum == NAN) return "";
                else if (_addent1 > _maxAddent || _addent1 < _minAddent || _addent2 > _maxAddent || _addent2 < _minAddent || _sum > _maxSum || _sum < -_minAddent) return "wrong input!";
                else if (_sum == _addent1 + _addent2)
                {
                    _newFlagSum = false; _newFlag1 = false; _newFlag2 = false;
                    /*_allHistory.Add(new(_addent1, _addent2, _sum))*/
                    ;
                    StreakCorrect++; StreakWrong = 0;
                    if (_decompositionLevel > 0 && StreakCorrect >= 20)
                    {
                        DecompositionLevel++; StreakCorrect = 0;
                        if (_decompositionLevel > 3)
                        {

                            SentrySdk.CaptureMessage("Win");
                            App.Current.MainPage.DisplayAlert("Win", "You Won!!", "OK");
                            return "YOU WON!!!!!!";
                        }
                    }
                    SentrySdk.CaptureMessage("Correct");
                    return "CORRECT :D";
                }
                else
                {
                    StreakWrong++;
                    if (_decompositionLevel > 0 && StreakWrong > 8)
                    {
                        DecompositionLevel--; StreakCorrect = 0; StreakWrong = 0;
                        if (_decompositionLevel == 0)
                        {
                            SentrySdk.CaptureMessage("Incorrect");

                            App.Current.MainPage.DisplayAlert("Lose", "You Lost!!", "OK");
                            return "YOU LOST!!!!!!";
                        }
                    }
                    SentrySdk.CaptureMessage("INCorrect");
                    return "WRONG :(";

                }
            }
        }

        public String SAddent1
        {
            get
            {
                if (_addent1 == NAN) return "";
                return _addent1.ToString();
            }
            set
            {
                try { Addent1 = Int32.Parse(value); OnPropertyChanged("_addent1"); } catch { _addent1 = NAN; }
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
                try { Addent2 = Int32.Parse(value); OnPropertyChanged("_addent2"); } catch { _addent2 = NAN; }
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
                try { Sum = Int32.Parse(value); OnPropertyChanged("_sum"); } catch { _sum = NAN; }
            }
        }


        public bool SumEnabled
        {
            get { return _newFlagSum; }
        }

        public bool Addent1Enabled
        {
            get { return _newFlag1; }
        }

        public bool Addent2Enabled
        {
            get { return _newFlag2; }
        }


    }
}