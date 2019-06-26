using System;
using System.Collections.Generic;
using System.Text;
using SpeechLib;

namespace SCTV
{
    public class Phrase
    {
        private float _accuracy;
        private int _accuracyLimit;
        private string _phrase;
        private string _ruleName;
        private ISpeechRecoResult _speechObject;
        private string _computerName;
        float _accuracyMax = 0.1F;						//to avoid divide by zero
        private int _numberOfElements = 0;

        public Phrase(ISpeechRecoResult heardSpeechObj, string computerName)
        {
            try
            {
                //calculate accuracy
                _accuracy = (float)heardSpeechObj.PhraseInfo.Elements.Item(0).EngineConfidence;

                //change accuracyMax dynamicly
                if (_accuracyMax < _accuracy)
                    _accuracyMax = _accuracy;

                if (_accuracy < 0)
                    _accuracy = 0;

                _accuracy = (int)((float)_accuracy / _accuracyMax * 100);

                //_computerName = Form1.computerNickName;
                _phrase = heardSpeechObj.PhraseInfo.GetText(0, -1, true);
                //_phrase = _phrase.Replace(_computerName, "").Trim();
                _phrase = _phrase.Replace(computerName, "").Trim();
                _ruleName = heardSpeechObj.PhraseInfo.Rule.Name;
                _speechObject = heardSpeechObj;
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Phrase", ex.Message);
            }            
        }

        /// <summary>
        /// The accuracy of the phrase
        /// </summary>
        public int Accuracy
        {
            get
            {
                return (int)_accuracy;
            }
            set
            {
                _accuracy = (float)value;
            }
        }

        /// <summary>
        /// The captured phrase
        /// </summary>
        public string phrase
        {
            get
            {
                return _phrase;
            }
            set
            {
                _phrase = value;
            }
        }

        /// <summary>
        /// Name of the rule for this phrase
        /// </summary>
        public string RuleName
        {
            get
            {
                return _ruleName;
            }
            set
            {
                _ruleName = value;
            }
        }

        /// <summary>
        /// Accuracy limit to make a match
        /// </summary>
        public int AcuracyLimit
        {
            get
            {
                return _accuracyLimit;
            }
            set
            {
                _accuracyLimit = value;
            }
        }

        /// <summary>
        /// The speech object that was recognized
        /// </summary>
        public ISpeechRecoResult SpeechObject
        {
            get
            {
                return _speechObject;
            }
            set
            {
                _speechObject = value;
            }
        }

        /// <summary>
        /// the name of the computer
        /// </summary>
        public string ComputerName
        {
            get
            {
                return _computerName;
            }
            set
            {
                _computerName = value;
            }
        }

        /// <summary>
        /// the name of the computer
        /// </summary>
        public int NumberOfElements
        {
            get
            {
                return this.SpeechObject.PhraseInfo.Rule.Children.Item(1).NumberOfElements;
            }
            set
            {
                _numberOfElements = value;
            }
        }
    }
}
