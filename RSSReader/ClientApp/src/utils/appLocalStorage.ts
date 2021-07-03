export const refreshTokenHeader = 'refreshToken'
export const refreshExpiresHeader = 'refreshTokenExpires'
export const authTokenHeader = 'authToken'
export const authExpiresHeader = 'authExpires'

export interface StorageTokenData {
  authToken: string
  authExpires: number
  refreshToken: string
  refreshExpires: number
}

export const saveTokenDataToStorage = (
  authToken: string,
  authExpires: number,
  refreshToken: string,
  refreshExpires: number
): void => {
  localStorage.setItem(authTokenHeader, authToken)
  localStorage.setItem(authExpiresHeader, authExpires.toString())
  localStorage.setItem(refreshTokenHeader, refreshToken)
  localStorage.setItem(refreshExpiresHeader, refreshExpires.toString())
}

export const getTokenDataFromStorage = (): StorageTokenData => {
  const authToken = localStorage.getItem(authTokenHeader)
  const authExpires = Number(localStorage.getItem(authExpiresHeader))
  const refreshToken = localStorage.getItem(refreshTokenHeader)
  const refreshExpires = Number(localStorage.getItem(refreshExpiresHeader))

  if (authToken == null) return null
  if (authExpires == null || authExpires == NaN) return null
  if (refreshToken == null) return null
  if (refreshExpires == null || refreshExpires == NaN) return null

  return ({
    authToken: authToken,
    authExpires: authExpires,
    refreshToken: refreshToken,
    refreshExpires: refreshExpires,
  } as unknown) as StorageTokenData
}

export const cleatTokenDataFromStorage = (): void => {
  localStorage.removeItem(authTokenHeader)
  localStorage.removeItem(refreshTokenHeader)
  localStorage.removeItem(refreshExpiresHeader)
}
