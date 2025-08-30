import { useState, useCallback } from 'react';
import { InteractionType, type PopupRequest } from '@azure/msal-browser';
import { useMsal, useMsalAuthentication } from "@azure/msal-react";

interface UseFetchWithMsalResult<T = any> {
    isLoading: boolean;
    error: any;
    data: T | null;
    execute: (method: 'GET' | 'POST' | 'PUT' | 'DELETE', endpoint: string, data?: Record<string, any> | null) => Promise<T | Response | undefined>;
}

//this is like a ready-to-use HTTP client wrapper with auth baked in.
const useFetchWithMsal = (msalRequest: PopupRequest): UseFetchWithMsalResult => {
    const { instance } = useMsal();
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<any>(null);
    const [data, setData] = useState<any>(null);

    const { result, error: msalError } = useMsalAuthentication(InteractionType.Popup, {
    ...msalRequest,
    account: instance.getActiveAccount() ?? undefined, // convert null to undefined
    // redirectUri: '/ask' ...if not set, it will use the one from msalConfig
    });

    const execute = async (
        method: 'GET' | 'POST' | 'PUT' | 'DELETE', 
        endpoint: string, 
        data?: Record<string, any> | null
    ): Promise<any> => {
        console.log("MSAL result:", result, "MSAL error:", msalError)
        if (msalError) {
            setError(msalError);
            return;
        }

        if (result) {
            try {
                const headers = new Headers();
                headers.append("Authorization", `Bearer ${result.accessToken}`);
                if (data) headers.append('Content-Type', 'application/json');

                const options: RequestInit = {
                    method,
                    headers,
                    body: data ? JSON.stringify(data) : null,
                };

                setIsLoading(true);
                
                const response = await fetch(endpoint, options);

                if (response.status === 200 || response.status === 201) {
                    let responseData: any = response;
                    try {
                        responseData = await response.json();
                    } catch (error) {
                        console.log(error);
                    } finally {
                        setData(responseData);
                        setIsLoading(false);
                        return responseData;
                    }
                }

                setIsLoading(false);
                return response;
            } catch (e) {
                setError(e);
                setIsLoading(false);
                throw e;
            }
        }
    };

    return {
        isLoading,
        error,
        data,
        execute: useCallback(execute, [result, msalError]),
    };
};

export default useFetchWithMsal;
