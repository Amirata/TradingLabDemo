//let accessToken: string | null = null;
//let refreshToken: string | null = null;

// If you prefer localStorage, uncomment these lines and adapt accordingly
//et accessToken = localStorage.getItem('accessToken');
//let refreshToken = localStorage.getItem('refreshToken');

export function setToken(token: string) {
  // accessToken = token;
  localStorage.setItem('accessToken', token);
}

export function getToken() {
  // return accessToken;
  return localStorage.getItem('accessToken');
}

export function setRefreshToken(token: string) {
  // refreshToken = token;
  localStorage.setItem('refreshToken', token);
}

export function getRefreshToken() {
  // return refreshToken;
  return localStorage.getItem('refreshToken');
}

export function clearTokens() {
  // accessToken = null;
  // refreshToken = null;
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
}
