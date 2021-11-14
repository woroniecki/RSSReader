import React from 'react'
import { authSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { getTokenDataFromStorage } from 'utils/appLocalStorage'

export interface useResetTokensProps {}

export const useResetTokens = (): void => {
  const dispatch = useAppDispatch()

  const MINUTE_MS = 60000
  React.useEffect(() => {
    const interval = setInterval(async () => {
      const tokenData = getTokenDataFromStorage()

      if (tokenData == null) {
        return
      }

      const utcDate = Date.now()

      if (tokenData.refreshExpires - utcDate < 0) {
        return
      }

      //if 90 seconds of activity of authtoken left, we should reset it
      if (tokenData.authExpires - utcDate > 90 * 1000) {
        return
      }

      dispatch(
        authSlice.refresh({
          refreshToken: tokenData.refreshToken,
          authToken: tokenData.authToken,
        })
      )
    }, MINUTE_MS)

    return () => clearInterval(interval) // This represents the unmount function, in which you need to clear your interval to prevent memory leaks.
  }, [])

  return
}

export default useResetTokens
