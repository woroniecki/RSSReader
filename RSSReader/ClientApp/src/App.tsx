import { createMuiTheme } from '@material-ui/core/styles'
import useRefreshToken from 'components/Auth/useRefreshToken'
import useResetTokens from 'components/Auth/useResetTokens'
import useGetBlogsAndSubs from 'components/Blog/useGetBlogsAndSubs'
import HomeApp from 'components/HomeApp'
import CustomizedSnackbar from 'components/Snackbar/CustomizedSnackbar'
import useResetLoaderSlice from 'components/Spinner/useResetLoaderSlice'
import UserApp from 'components/UserApp'
import React from 'react'
import { useSelector } from 'react-redux'
import { authSlice, layoutSlice, snackbarSlice } from 'store/slices'
import {} from 'styled-components'
import './App.css'
import AppSpinner from './components/Spinner/AppSpinner'

interface Props {
  /**
   * Injected by the documentation to work in an iframe.
   * You won't need it on your project.
   */
  window?: () => Window
}

function App(props: Props) {
  const snackbar = useSelector(snackbarSlice.stateSelector)
  const { userName } = useSelector(authSlice.stateSelector)
  const { loader } = useSelector(layoutSlice.stateSelector)
  useRefreshToken()
  useResetTokens()
  useGetBlogsAndSubs()
  useResetLoaderSlice()

  const [mode, setMode] = React.useState<'light' | 'dark'>('dark')

  const theme = React.useMemo(
    () =>
      createMuiTheme({
        palette: {
          type: mode,
          background: {
            default: '#2B2B2B',
            paper: '#262626',
          },
          divider: '#000000',
        },
      }),
    [mode]
  )

  const GetContent = () => {
    if (!userName) {
      return <HomeApp />
    } else {
      return <UserApp />
    }
  }

  return (
    <>
      {GetContent()}
      {loader != layoutSlice.type.none && <AppSpinner />}
      {snackbar.open && <CustomizedSnackbar />}
    </>
  )
}

export default App
