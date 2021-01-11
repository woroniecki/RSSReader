export const refreshTokenHeader = 'refreshToken'
export const refreshTokenExpiresHeader = 'refreshTokenExpires'
export const authTokenHeader = 'authToken'

export interface StorageTokenData {
  authToken: string
  refreshToken: string
  refreshExpires: number
}

export const saveTokenDataToStorage = (
  authToken: string,
  refreshToken: string,
  refreshTokenExpires: number
) => {
  localStorage.setItem(authTokenHeader, authToken)
  localStorage.setItem(refreshTokenHeader, refreshToken)
  localStorage.setItem(
    refreshTokenExpiresHeader,
    refreshTokenExpires.toString()
  )
}

export const getTokenDataFromStorage = () => {
  const authToken = localStorage.getItem(authTokenHeader)
  const refreshToken = localStorage.getItem(refreshTokenHeader)
  const refreshTokenExpires = Number(
    localStorage.getItem(refreshTokenExpiresHeader)
  )

  if (authToken == null) return null
  if (refreshToken == null) return null
  if (refreshTokenExpires == null || refreshTokenExpires == NaN) return null

  return ({
    authToken: authToken,
    refreshToken: refreshToken,
    refreshTokenExpires: refreshTokenExpires,
  } as unknown) as StorageTokenData
}

export const cleatTokenDataFromStorage = () => {
  localStorage.removeItem(authTokenHeader)
  localStorage.removeItem(refreshTokenHeader)
  localStorage.removeItem(refreshTokenExpiresHeader)
}
