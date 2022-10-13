using System.Diagnostics;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Serilog;

namespace Test.Rest.Interceptors
{
    public class ClientInvocationInterceptor : Interceptor
    {
        private Stopwatch? _sw;

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            _sw = Stopwatch.StartNew();

            var call = continuation(request, context);

            return new AsyncUnaryCall<TResponse>(
                HandleResponse(call.ResponseAsync, MethodType.Unary, context.Method.ToString()),
                call.ResponseHeadersAsync,
                call.GetStatus, call.GetTrailers, call.Dispose);
        }

        private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> inner, MethodType type,
            string? methodName)
        {
            try
            {
                return await inner;
            }
            catch (Exception ex)
            {
                Log.Error("Error receiving call. Type: {MethodType}. Method: {Method}. Exception: {ex}", type,
                    methodName,
                    ex);
                throw new InvalidOperationException("Custom error", ex);
            }
            finally
            {
                _sw?.Stop();
                if (_sw != null)
                    Log.Debug("Finishing receiving call. Type: {MethodType}. Method: {Method}. Time: {time} ms", type,
                        methodName, _sw.Elapsed.TotalMilliseconds);
            }
        }
    }
}