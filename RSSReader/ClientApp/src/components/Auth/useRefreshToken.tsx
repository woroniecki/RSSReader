import React from 'react'
import { useAppDispatch } from 'store/store'
import { authSlice, layoutSlice } from 'store/slices'
import {
  getTokenDataFromStorage,
  saveTokenDataToStorage,
} from 'utils/appLocalStorage'

export interface useRefreshTokenProps {}

export const useRefreshToken = (): void => {
  const dispatch = useAppDispatch()

  React.useEffect(() => {
    const refreshToken = async () => {
      dispatch(layoutSlice.actions.setLoader(layoutSlice.type.fullScreen))

      await dispatch(
        authSlice.refresh({
          refreshToken: tokenData.refreshToken,
          authToken: tokenData.authToken,
        })
      )

      dispatch(layoutSlice.actions.setLoader(layoutSlice.type.none))
    }

    const tokenData = getTokenDataFromStorage()

    if (tokenData == null) {
      return
    }

    if (tokenData.refreshExpires > Date.now()) {
      saveTokenDataToStorage('', -1, '', -1)
      refreshToken()
    }
  }, [])

  return
}

export default useRefreshToken
