using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Sevices.Interfaces
{
    public interface ITestingPlatform
    {
        ITestManager TestManager { get; }
        ITestQuestionManager TestQuestionManager { get; }
        ITestAnswerManager TestAnswerManager { get; }
        ITestResultManager TestResultManager { get; }
        IRandomStringGenerator RandomStringGenerator { get; }
        IAnswerImageManager AnswerImageManager { get; }
        ITestReportManager TestReportManager { get; }
        ITestMarkManager TestMarkManager { get; }
    }
}
