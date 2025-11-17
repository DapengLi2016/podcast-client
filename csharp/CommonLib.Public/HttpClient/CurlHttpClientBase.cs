//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using Flurl.Http;
using Flurl.Util;
using Microsoft.SpeechServices.CommonLib.HttpClient;
using Microsoft.SpeechServices.Cris.Http.DTOs.Public;
using Microsoft.SpeechServices.DataContracts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.SpeechServices.CommonLib.Util;

public abstract class CurlHttpClientBase<TDto> : HttpClientBase
    where TDto : StatefulResourceBase
{
    public CurlHttpClientBase(HttpClientConfigBase config)
        : base(config)
    {
    }

    protected async Task<(TDto response, IReadOnlyNameValueList<string> headers)> CreateDtoAsync(
        TDto dto,
        string operationId)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var (responseString, headers) = await CreateDtoWithStringResponseAsync(
            dto: dto,
            operationId: operationId).ConfigureAwait(false);
        var typedResponse = JsonConvert.DeserializeObject<TDto>(responseString);
        return (typedResponse, headers);
    }

    protected async Task<(string responseString, IReadOnlyNameValueList<string> headers)> CreateDtoWithStringResponseAsync(
        TDto dto,
        string operationId)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var responseTask = CreateDtoWithResponseAsync(
            dto: dto,
            operationId: operationId);
        var response = await responseTask.ConfigureAwait(false);
        var stringResponse = await response.GetStringAsync()
            .ConfigureAwait(false);
        return (stringResponse, response.Headers);
    }

    protected async Task<TDto> CreateDtoAndWaitUntilTerminatedAsync(
        TDto dto)
    {
        Console.WriteLine($"Creating resource {dto.Id} :");

        var operationId = Guid.NewGuid().ToString();
        var (response, createResponseHeaders) = await CreateDtoAsync(
            dto: dto,
            operationId: operationId).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(response);

        if (!createResponseHeaders.TryGetFirst(CommonPublicConst.Http.Headers.OperationLocation, out var operationLocation) ||
            string.IsNullOrEmpty(operationLocation))
        {
            throw new InvalidDataException($"Missing header {CommonPublicConst.Http.Headers.OperationLocation} in headers");
        }

        var operationClient = new OperationClient(this.SpeechConfig);

        await operationClient.QueryOperationUntilTerminateAsync(new Uri(operationLocation)).ConfigureAwait(false);

        return await GetTypedDtoAsync(
            translationId: response.Id).ConfigureAwait(false);
    }

    protected async Task<PaginatedResources<TDto>> ListTypedDtosAsync()
    {
        var url = await this.BuildRequestBaseAsync().ConfigureAwait(false);

        return await RequestWithRetryAsync(async () =>
        {
            return await url.GetAsync()
                .ReceiveJson<PaginatedResources<TDto>>()
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    protected async Task<TDto> GetTypedDtoAsync(string translationId)
    {
        var response = await GetDtoResponseAsync(translationId).ConfigureAwait(false);

        // Not exist.
        if (response == null)
        {
            return null;
        }

        return await response.GetJsonAsync<TDto>().ConfigureAwait(false);
    }

    protected async Task<string> GetDtoResponseStringAsync(string id)
    {
        var response = await GetDtoResponseAsync(id).ConfigureAwait(false);
        return await response.GetStringAsync().ConfigureAwait(false);
    }

    protected async Task<IFlurlResponse> GetDtoResponseAsync(string id)
    {
        var url = await this.BuildRequestBaseAsync().ConfigureAwait(false);

        url = url.AppendPathSegment(id);

        return await RequestWithRetryAsync(async () =>
        {
            try
            {
                return await url
                    .GetAsync()
                    .ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                if (ex.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }

                Console.Write($"Response failed with error: {await ex.GetResponseStringAsync().ConfigureAwait(false)}");
                throw;
            }
        }).ConfigureAwait(false);
    }

    private async Task<IFlurlResponse> CreateDtoWithResponseAsync(
        TDto dto,
        string operationId)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentException.ThrowIfNullOrEmpty(dto.Id);
        ArgumentException.ThrowIfNullOrEmpty(operationId);

        var url = await this.BuildRequestBaseAsync().ConfigureAwait(false);
        url = url.AppendPathSegment(dto.Id)
            .WithHeader(CommonPublicConst.Http.Headers.OperationId, operationId);

        return await RequestWithRetryAsync(async () =>
        {
            return await url
                .PutJsonAsync(dto)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
