using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using PartPartWhole.Models;
using Sentry;
using System.Collections;

namespace PartPartWhole.ViewModels
{
    public partial class PartPartWholeViewModel : ObservableObject
    {
        private readonly bool ASSERT=false;
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
        [AlsoNotifyChangeFor(nameof(History))]
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




        
       
        //internal List<PPWObject> AllHistory { get => allHistory; set => allHistory = value; }

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
        public bool _settingsVisible = true;
        [ObservableProperty]
        private bool _requireNewAddents = true;
        //TODO: do it more elegant code with only one propety(without _settingsVisible)
        public bool RequireNewAddents2 { get { return _requireNewAddents; } set { this.Default(); _requireNewAddents = value; IsDecomposition = false; SettingsVisible = !value;  } } 

        private bool _freeCombination = false;
        private List<PPWObject> _allHistory = new();
        private List<int> _impossibleSums = new();

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
                    if (_decompositionLevel == -1) DecompositionLevel = 2;
                    FMustFindOneTwoBoth = 1;
                    FMustFindTheSum = false;
                    MaxAddent = 20;
                    MinAddent = 1;
                    MaxSum = 20;
                    FInsisitentOnOne = false;

                }

                //OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotDecomposition));
            }
        }
        public bool IsNotDecomposition { get { return !IsDecomposition; } }

        //TODO:make a nice Enum
        public String TrueStatement
        {
            get
            {
               
                if (_addent1 == NAN || _addent2 == NAN || _sum == NAN) return "";
                else if (_addent1 > _maxAddent || _addent1 < _minAddent || _addent2 > _maxAddent || _addent2 < _minAddent || _sum > _maxSum || _sum < _minAddent) return "wrong input!";
                else if (_sum == _addent1 + _addent2)
                {
                    if(_requireNewAddents)
                    {
                        foreach (PPWObject ppw in _allHistory)
                            if (ppw.Sum == _sum && ppw.Addent1 == _addent1) { 
                               
                                Addent1 = NAN; Addent2 = NAN;  
                                return "Find NEW combination";}

                    }
                    OnPropertyChanged(nameof(SumEnabled)); OnPropertyChanged(nameof(Addent1Enabled)); OnPropertyChanged(nameof(Addent2Enabled));
                    _allHistory.Add(new PPWObject(_addent1, _addent2, _sum));
                                        
                    if (_decompositionLevel > 0)
                    {
                        StreakCorrect++; StreakWrong = 0;
                        if(StreakCorrect >= 20)
                        {
                            DecompositionLevel++; StreakCorrect = 0;
                            if (_decompositionLevel > 3)
                            {

                                if(ASSERT) SentrySdk.CaptureMessage("Win");
                                App.Current.MainPage.DisplayAlert("Win", "You Won!!", "OK");
                                return "YOU WON!!!!!!";
                            }
                        }
                    }
                    if (ASSERT) SentrySdk.CaptureMessage("Correct");
                    SentrySdk.CaptureMessage(string.Format("  Correct: {0}={1}+{2}", _sum, _addent1, _addent2));
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
                                if (ASSERT) SentrySdk.CaptureMessage("Lose");

                                App.Current.MainPage.DisplayAlert("Lose", "You Lost!!", "OK");
                                return "YOU LOST!!!!!!";
                            }
                        }
                    }
                    if (ASSERT) SentrySdk.CaptureMessage("Incorrect");
                    SentrySdk.CaptureMessage(string.Format("Incorrect: {0}={1}+{2}", _sum, _addent1, _addent2));
                    return "WRONG :(";
                }
            }
        }

        public String History
        {
            get {
                String s = "";
                if (_requireNewAddents && _sum != NAN)
                {
                    s = "HISTORY:\n";
                    foreach (PPWObject ppw in _allHistory)
                        if (ppw.Sum == Sum)
                            s += ppw.Addent1 + "\t" + ppw.Addent2 + "\n";
                }
                
                return s;
                //return allHistory;
            }
        }

        private int GenerateNewAddent(int newSum)
        {
            ArrayList possibleAddents = new();
            for(int i=Math.Max(_minAddent, newSum-_maxAddent);i<=Math.Min(_maxAddent, newSum-_minAddent);i++)
            {
                bool isExist = false;
                foreach(PPWObject ppw in _allHistory)
                    if(ppw.Sum==newSum && ppw.Addent1==i) isExist = true;
                if (!isExist)
                    possibleAddents.Add(i);
            }
            if(possibleAddents.Count > 0) { Random r = new(); return (int)possibleAddents[r.Next(possibleAddents.Count)]; }

            if(!_impossibleSums.Contains(newSum)) _impossibleSums.Add(newSum);
            return NAN;
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
            if (ASSERT)
                SentrySdk.CaptureMessage("Hello Sentry");
            if (_decompositionLevel == 1) { MinAddent = 0; MaxAddent = 10; MaxSum = 10; FInsisitentOnOne = true; }
            if (_decompositionLevel == 2) { MinAddent = 0; MaxAddent = 20; MaxSum = 20; FInsisitentOnOne = false; }
            if (_decompositionLevel == 3) { MinAddent = 0; MaxAddent = 100; MaxSum = 100; }
            //TODO: validation also in the form with Binding
            if (_minAddent < 0) MinAddent = 0;
            if (_maxAddent < _minAddent + 3) MaxAddent = _minAddent + 2;
            if (_maxSum > 2 * _maxAddent || _maxSum<=2*_minAddent) MaxSum = 2 * _maxAddent;

           
            int[] factors = new int[3];
            Random r = new();
            factors[2] = r.Next(2 * _minAddent, _maxSum + 1);
            if (_fInsisitentOnOne) factors[2] = _lastNum;
            factors[0] = r.Next(_minAddent, Math.Min(_maxAddent, factors[2])+1);
            factors[1] = factors[2] - factors[0];

            if (ASSERT)
                SentrySdk.CaptureMessage("First factors success");

            
            
            if (_decompositionLevel > 1)
            {
                
                if (_sum != _addent1 + _addent2) StreakWrong++;//you moved next without solving
                int minSum = (_decompositionLevel >= 3) ? 20 : 10; 
                factors[2] = r.Next(Math.Max(_minAddent, minSum), _maxSum);
                while (factors[2]%10==9) factors[2] = r.Next(Math.Max(_minAddent, minSum), _maxSum);
                if (factors[2] % 10 == 0) factors[0]= r.Next(_minAddent, Math.Min(_maxAddent+ 1, factors[2]) );
                else { 
                    
                    int tens = r.Next(Math.Max(_minAddent / 10, 0), factors[2] / 10 - 1);
                    int ones = r.Next(factors[2] % 10 + 1, 10);
                    factors[0] = tens * 10 + ones;
                }
                factors[1] = factors[2] - factors[0];
            }
            if (ASSERT)
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
            if (ASSERT)
                SentrySdk.CaptureMessage("Xs success");

            if(_requireNewAddents)
            {
                //make some win message before arriving to it
                if (_impossibleSums.Count >= _maxSum - 2 * _minAddent - 1) { 
                    _impossibleSums.Clear(); _allHistory.Clear(); _freeCombination = !_freeCombination; }
                factors[0] = GenerateNewAddent(factors[2]);
                if (_freeCombination) factors[0] = NAN;
                while (_impossibleSums.Contains(factors[2]))
                {
                    if (_impossibleSums.Count >= _maxSum - 2 * _minAddent - 1) { 
                        _impossibleSums.Clear(); _allHistory.Clear(); _freeCombination = !_freeCombination; }
                    factors[2] = r.Next(2 * _minAddent, _maxSum + 1);
                    factors[0] = GenerateNewAddent(factors[2]);
                    if (_freeCombination) factors[0] = NAN;
                }
                OnPropertyChanged(nameof(History));

            }
            

            Addent1 = factors[0];
            Addent2 = factors[1];
            Sum = factors[2];

            SentrySdk.CaptureMessage(string.Format("Question:{0}={1}+{2}", SSum, SAddent1, SAddent2));
            
            if (ASSERT)
                SentrySdk.CaptureMessage("Pulling the entries success");

            

        }

    }
}