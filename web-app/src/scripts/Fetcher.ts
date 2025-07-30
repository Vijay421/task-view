// TODO: change any to generic types.
// TODO: support mocking.
export default class Fetcher {
    private _url: string;
    private _method: string;
    private _headers: HeadersInit;
    private _requestParams: RequestInit;
    private _dryRun: boolean;
    private _onOk: ((response: any) => void) | undefined;
    private _onNonOk: ((response: any) => void) | undefined;
    private _onStatusCodeMap: Map<number, (response: any) => void>;
    private _onError: ((error: unknown) => void) | undefined;
    private _onParseResponse: ((response: Response) => Promise<any>) | undefined;

    constructor(url: string, method: string, headers: HeadersInit, requestParams: RequestInit, dryRun: boolean = false) {
        this._url = url;
        this._method = method;
        this._headers = headers;
        this._requestParams = requestParams;
        this._onStatusCodeMap = new Map();
        this._dryRun = dryRun;
    }

    static get(url: string, headers: HeadersInit = {}, requestParams: RequestInit = {}) {
        return new Fetcher(url, "GET", headers, requestParams);
    }

    static post(url: string, headers: HeadersInit = {}, requestParams: RequestInit = {}) {
        return new Fetcher(url, "POST", headers, requestParams);
    }

    setOnOk(onOk: (response: any) => void): Fetcher {
        this._onOk = onOk;

        return this;
    }

    setOnNonOk(onNonOk: (response: any) => void): Fetcher {
        this._onNonOk = onNonOk;

        return this;
    }

    setOnStatusCode(statusCode: number, onStatusCode: (response: any) => void): Fetcher {
        this._onStatusCodeMap.set(statusCode, onStatusCode);

        return this;
    }

    setOnError(onError: (error: unknown) => void): Fetcher {
        this._onError = onError;

        return this;
    }

    setParseResponse(onParseResponse: (response: Response) => Promise<any>): Fetcher {
        this._onParseResponse = onParseResponse;

        return this;
    }

    async fetch<T extends object>(payload?: T) {
        const request = {
            method: this._method,
            body: payload ? JSON.stringify(payload) : undefined,
            ...this._requestParams,
        };
        request.headers = this._headers;

        if (this._dryRun) return;

        try {
            const response = await fetch(this._url, request);

            let parsedResponse = undefined;
            if (this._onParseResponse)
                parsedResponse = await this._onParseResponse(response);

            if (response.ok) {
                if (this._onOk) this._onOk(parsedResponse || response);
            } else {
                if (this._onNonOk) this._onNonOk(parsedResponse || response);
            }

            const onStatusCode = this._onStatusCodeMap.get(response.status);
            if (onStatusCode)
                onStatusCode(response);
        } catch(error) {
            if (this._onError) this._onError(error);
        }
    }
}
