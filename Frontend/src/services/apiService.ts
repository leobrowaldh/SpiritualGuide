

// const API_BASE_URL =  import.meta.env.VITE_API_BASE_URL;
// async function fetchApi<T>(url: string, token: string, body?: any): Promise<T> {
//   const response = await fetch(url, {
//     method: 'POST',
//     headers: {
//       'Content-Type': 'application/json',
//       Authorization: `Bearer ${token}`,
//     },
//     body: JSON.stringify(body),
//   });
//   if (!response.ok) {
//     throw new Error('API request failed');
//   }
//   return response.json() as Promise<T>;
// }

// export function askQuestion(token: string, question: string): Promise<AskResponse> {
//   return fetchApi<AskResponse>(`${API_BASE_URL}/Ask`, token, { question });
// }

// //this one will be called from an admin page
// export function addQuote(token: string, quote: string): Promise<AddQuoteResponse> {
//   return fetchApi<AddQuoteResponse>(`${API_BASE_URL}/AddQuote`, token, { quote });
// }
