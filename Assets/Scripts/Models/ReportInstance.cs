using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.QuestionDisplayer;

namespace Assets.Scripts.Models
{
    public class ReportInstance
    {
        public string UserName;
        public string TakenAt;//Not gonna use DateTime , yeah
        public SectionType SectionType;
        public WeighType WeighType;
        public List<Question> Questions;
    }
    public enum WeighType
    {
        NaN,
        Obese,
        NonObese
    }
}
