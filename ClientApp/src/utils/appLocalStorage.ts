export const refreshTokenHeader = 'refreshToken'
export const refreshExpiresHeader = 'refreshTokenExpires'
export const authTokenHeader = 'authToken'

export interface StorageTokenData {
  authToken: string
  refreshToken: string
  refreshExpires: number
}

export const saveTokenDataToStorage = (
  authToken: string,
  refreshToken: string,
  refreshExpires: number
) => {
  localStorage.setItem(authTokenHeader, authToken)
  localStorage.setItem(refreshTokenHeader, refreshToken)
  localStorage.setItem(refreshExpiresHeader, refreshExpires.toString())
}

export const getTokenDataFromStorage = () => {
  const authToken = localStorage.getItem(authTokenHeader)
  const refreshToken = localStorage.getItem(refreshTokenHeader)
  const refreshExpires = Number(localStorage.getItem(refreshExpiresHeader))

  if (authToken == null) return null
  if (refreshToken == null) return null
  if (refreshExpires == null || refreshExpires == NaN) return null

  return ({
    authToken: authToken,
    refreshToken: refreshToken,
    refreshExpires: refreshExpires,
  } as unknown) as StorageTokenData
}

export const cleatTokenDataFromStorage = () => {
  localStorage.removeItem(authTokenHeader)
  localStorage.removeItem(refreshTokenHeader)
  localStorage.removeItem(refreshExpiresHeader)
}
