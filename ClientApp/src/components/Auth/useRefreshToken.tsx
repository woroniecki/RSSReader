import { LoginResponse, Token } from 'api/api.types'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { authSlice } from 'store/slices'
import { getTokenDataFromStorage } from 'utils/appLocalStorage'

export interface useRefreshTokenProps {}

export const useRefreshToken = () => {
  const dispatch = useAppDispatch()

  React.useEffect(() => {
    const tokenData = getTokenDataFromStorage()

    if (tokenData == null) {
      return
    }

    if (tokenData.refreshExpires > Date.now()) {
      const promise = dispatch(
        authSlice.refresh({
          refreshToken: tokenData.refreshToken,
          authToken: tokenData.authToken,
        })
      )
    }
  }, [])

  return
}

export default useRefreshToken
