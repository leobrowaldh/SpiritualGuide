import { useMsal } from "@azure/msal-react";
import axios from "axios";
import { type AccountInfo } from "@azure/msal-browser";

export const useApiClient = () => {
    const { instance: msalInstance } = useMsal();
    const API_URL = import.meta.env.VITE_API_BASE_URL;
    const apiClientId = import.meta.env.VITE_API_CLIENT_ID;
    const DEFAULT_SCOPES = [`api://${apiClientId}/access_as_user`];

    const apiClient = axios.create({ baseURL: API_URL });

    apiClient.interceptors.request.use(async (config) => {
        const account: AccountInfo | null = msalInstance.getActiveAccount();
        if (account) {
            const tokenResponse = await msalInstance.acquireTokenSilent({
                account,
                scopes: DEFAULT_SCOPES,
            });
            config.headers.Authorization = `Bearer ${tokenResponse.accessToken}`;
        }
        return config;
    });

    return { apiClient };
};
