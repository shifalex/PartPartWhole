using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Toolkit.Mvvm;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace PartPartWhole.ViewModels
{
    public partial class PartPartWholeViewModel : ObservableObject
	{
		
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

        private readonly int NAN = -1111;

        [ObservableProperty]
        private int _fMustFindOneTwoBoth = 2;
        [ObservableProperty]
        private bool _fMustFindTheSum = true;
        [ObservableProperty]
        private int _maxAddent = 5;
        [ObservableProperty]
        private int _minAddent =0;
        [ObservableProperty]
        private int _maxSum = 10;

        [ObservableProperty]
        private bool _fInsisitentOnOne = false;
        private int _lastNum = 10;


        [ICommand]
        public void GenerateExercise()
        {
            if (_maxSum > 2 * _maxAddent) _maxSum = 2 * _maxAddent;


            int[] factors = new int[3];
            Random r = new();
            factors[0]= r.Next(_minAddent, _maxAddent+1);
            factors[2]= r.Next(2*_minAddent, _maxSum + 1);
            while (factors[2] < factors[0] || (factors[2] - factors[0]) > _maxAddent || (factors[2] - factors[0]) < _minAddent)
                factors[2] = r.Next(0, _maxSum+1);

            //TODO: FIX make an entry near checkbox which will be enabled only when checked
            if (_fInsisitentOnOne)
            {
                factors[2] = _lastNum;
                while (factors[2] < factors[0] || (factors[2] - factors[0]) > _maxAddent || (factors[2] - factors[0]) < _minAddent)
                    factors[0] = r.Next(_minAddent, _maxAddent + 1);
            }

            factors[1] = (factors[2] - factors[0]);

            int QuestionType = r.Next(2);
            if (_fMustFindOneTwoBoth == 1) QuestionType = 1;
            if (_fMustFindOneTwoBoth == 2) QuestionType = 2;
            int n = r.Next(3);
            if (_fMustFindTheSum) n = 2;
            if (QuestionType == 1)
                factors[n] = NAN;
            else
                for (int i = 0; i < 3; i++)
                    if (i != n) factors[i] = NAN;
            
            
            _newFlag1 = (factors[0]== NAN);
            _newFlag2 = (factors[1] == NAN);
            _newFlagSum = (factors[2] == NAN);

            Addent1 = factors[0];
            Addent2 = factors[1];
            Sum = factors[2];

        }

        public String TrueStatement {
            get
            {
                
                if (_addent1 == NAN || _addent2 == NAN || _sum == NAN) return "";
                if (_addent1 > _maxAddent || _addent1 < _minAddent || _addent2 > _maxAddent || _addent2 < _minAddent || _sum > _maxSum || _sum < -_minAddent) return "wrong input!";
                if (_sum == _addent1 + _addent2) { _newFlagSum = false; _newFlag1 = false; _newFlag2 = false; return "CORRECT :D"; }
                else return "WRONG :(";
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
            get {   return _newFlagSum; }
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