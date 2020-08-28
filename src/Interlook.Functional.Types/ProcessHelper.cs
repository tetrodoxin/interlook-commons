#region license

//MIT License

//Copyright(c) 2013-2020 Andreas Hübner

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

#endregion 
using Interlook.Monads;
using Interlook.Text;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Interlook.Functional.Types
{
    /// <summary>
    /// Helper class for starting external processes.
    /// </summary>
    public class ProcessHelper
    {
        private const int _processTimout = 10000;
        private const string CmdExecutableFilename = "cmd.exe";

        private TaskCompletionSource<bool> _errorCloseEvent = new TaskCompletionSource<bool>();
        private List<string> _errors = new List<string>();
        private TaskCompletionSource<object> _exitEvent = new TaskCompletionSource<object>();
        private bool _hasRun = false;
        private List<string> _output = new List<string>();
        private TaskCompletionSource<bool> _outputCloseEvent = new TaskCompletionSource<bool>();
        private Process _process;
        private List<Task> _processTasks = new List<Task>();

        /// <summary>
        /// Timeout for executing the process.
        /// </summary>
        internal virtual int ProcessTimout => _processTimout;

        internal ProcessHelper(ProcessStartInfo info)
        {
            _process = new Process() { StartInfo = info };

            _process.Exited += handleExitEvent;
            _process.OutputDataReceived += handleOutputDataReceived;
            _process.ErrorDataReceived += handleErrorDataReceived;
            _process.EnableRaisingEvents = true;

            _processTasks.Add(_exitEvent.Task);
            _processTasks.Add(_outputCloseEvent.Task);
            _processTasks.Add(_errorCloseEvent.Task);
        }

        /// <summary>
        /// Starts an external process, specified by a <see cref="ProcessStartInfo"/> object
        /// returned by a factory method .
        /// </summary>
        /// <param name="infoFactory">The factory method of the object, that contains information of the process to start.</param>
        /// <returns>Either an instance of <see cref="Right{Exception, IProcessResult}"/> containing an object 
        /// implementing <see cref="IProcessResult"/>, if the execution succeeded without exceptions; 
        /// otherwise an <see cref="Left{Exception, IProcessResult}"/> with the occured exception.</returns>
        public static Either<Exception, IProcessResult> Start(Func<ProcessStartInfo> infoFactory)
            => infoFactory.ToExceptionEither()
                .FailIf(_ => infoFactory == null, new ArgumentNullException(nameof(infoFactory)))
                .Bind(_ => Try.Invoke(() => new ProcessHelper(infoFactory())).ToExceptionEither())
                .Bind(p => p.Start());

        /// <summary>
        /// WINDOWS ONLY! Starts an external process, wrapped in an call of <c>cmd.exe</c>
        /// </summary>
        /// <param name="executablePath">The path of the executable to start.</param>
        /// <param name="workingDir">The directory that will be used as starting environment.</param>
        /// <param name="parametersString">All command-line arguments for the executable as a single string.</param>
        /// <returns>
        /// Either an instance of <see cref="Right{Exception, IProcessResult}" /> containing an object
        /// implementing <see cref="IProcessResult" />, if the execution succeeded without exceptions;
        /// otherwise an <see cref="Left{Exception, IProcessResult}" /> with the occured exception.
        /// </returns>
        public static Either<Exception, IProcessResult> StartWindowsCmdWrapper(string executablePath, string workingDir, string parametersString)
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Try.Invoke(() => new ProcessHelper(createCmdWrapperStartInfo(executablePath, workingDir, parametersString))).ToExceptionEither()
                    .Bind(p => p.Start())
                : Either.Left<Exception, IProcessResult>(new PlatformNotSupportedException($"{nameof(StartWindowsCmdWrapper)}() is supported on Microsoft-Windows only."));


        internal Either<Exception, IProcessResult> Start()
            => checkCanStart()
                .Bind(StartProcessInternal)
                .Bind(beginAsyncHandlingSafe)
                .Bind(waitForProcessFinish);

        internal Either<Exception, Process> StartProcessInternal(Process process)
            => Try.InvokeToExceptionEither(() => process.Start())
                .AddOuterException(ex => new Exception("Process could not be started.", ex))
                .Select(_ => process);

        private static Process beginAsyncHandling(Process process)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }

        private static ProcessStartInfo createCmdWrapperStartInfo(string processExecutablePath, string commandParametersString)
                                    => createCmdWrapperStartInfo(processExecutablePath, System.IO.Path.GetDirectoryName(processExecutablePath) ?? string.Empty, commandParametersString);

        private static ProcessStartInfo createCmdWrapperStartInfo(string processExecutablePath, string workingDir, string commandParametersString)
        {
            return new ProcessStartInfo()
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = CmdExecutableFilename,
                Arguments = $"/c \"\"{processExecutablePath}\" {commandParametersString}",
                WorkingDirectory = workingDir,
            };
        }
        private static void handleDataReceivedEvent(DataReceivedEventArgs e, TaskCompletionSource<bool> closeEvent, List<string> outputList)
        {
            try
            {
                if (e.Data == null)
                {
                    closeEvent.SetResult(true);
                }
                else
                {
                    outputList.Add(e.Data);
                }
            }
            catch
            { }
        }

        private bool atomicToggleRunState()
            => _hasRun
                ? false
                : (_hasRun = true) == true;

        private Either<Exception, Process> beginAsyncHandlingSafe(Process process)
            => Try.InvokeToExceptionEither(() => beginAsyncHandling(process));

        private Either<Exception, Process> checkCanStart()
                                    => _process.ToExceptionEither()
                .FailIf(_ => !atomicToggleRunState(), new InvalidOperationException("Process has already run, cannot start again."));

        private string getErrorsAsSingleString()
        {
            try
            {
                var sb = new StringBuilder();
                foreach (var line in _errors)
                {
                    if (line.IsNeitherNullNorEmpty()) sb.AppendLine(line);
                }

                return sb.Length > 1
                    ? $"{Environment.NewLine}Error-Output:{Environment.NewLine}{sb.ToString()}"
                    : string.Empty;
            }
            catch
            {
                return "<Getting error output failed.>";
            }
        }

        private void handleErrorDataReceived(object? sender, DataReceivedEventArgs e) => handleDataReceivedEvent(e, _errorCloseEvent, _errors);

        private void handleExitEvent(object? sender, EventArgs e)
        {
            try
            {
                _exitEvent.SetResult(true);
            }
            catch
            { }
        }

        private void handleOutputDataReceived(object? sender, DataReceivedEventArgs e) => handleDataReceivedEvent(e, _outputCloseEvent, _output);

        private Either<Exception, IProcessResult> waitForProcessFinish(Process process)
            => Try.InvokeToExceptionEither(() => Task.WhenAll(_processTasks))   // create a task, that waits for all local eventsources
                .Bind(processCompletionTask => Try.InvokeToExceptionEither(() 
                    => Task.WaitAny(Task.Delay(ProcessTimout), processCompletionTask) == 1))    // index 1 is the correct/expected task to finish (not delay task)
                .FailIf(exitedNormally => !exitedNormally,
                    Try.InvokeToExceptionEither(() => { process.Kill(); return Unit.Default; }) // obviously the delay task finished, so kill our process
                    .MapEither(
                        ex => new Exception($"Error killing not responding process. {getErrorsAsSingleString()}", ex),
                        _ => new Exception($"Non responding process had to be killed. {getErrorsAsSingleString()}")))
                .Select(_ => process.ExitCode)
                .Bind(exitCode => ProcessResult.Create(exitCode == 0, _errors, _output));
    }
}