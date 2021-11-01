import './App.css'
import React from 'react'
import clsx from 'clsx'
import {} from 'styled-components'
import CssBaseline from '@material-ui/core/CssBaseline'
import {
  makeStyles,
  useTheme,
  createMuiTheme,
  ThemeProvider,
} from '@material-ui/core/styles'
import AppHome from 'components/AppHome'
import AppNavbar from 'components/AppNavbar/AppNavbar'
import Login from 'components/Auth/Login'
import Register from 'components/Auth/Register'
import { Switch, Route } from 'react-router'
import AppSpinner from './components/Spinner/AppSpinner'
import { useSelector } from 'react-redux'
import { layoutSlice, navbarSlice, snackbarSlice } from 'store/slices'
import useRefreshToken from 'components/Auth/useRefreshToken'
import useResetTokens from 'components/Auth/useResetTokens'
import useGetBlogsAndSubs from 'components/Blog/useGetBlogsAndSubs'
import useResetLoaderSlice from 'components/Spinner/useResetLoaderSlice'
import SingleBlog from 'components/Blog/SingleBlog'
import SingleArticle from 'components/Article/SingleArticle'
import { useAppDispatch } from 'store/store'
import { Container } from '@material-ui/core'
import Footer from 'components/Footer/Footer'
import CustomizedSnackbar from 'components/Snackbar/CustomizedSnackbar'
import UserNavbar from 'components/AppNavbar/UserNavbar'

interface Props {
  /**
   * Injected by the documentation to work in an iframe.
   * You won't need it on your project.
   */
  window?: () => Window
}

function App(props: Props) {
  const { loader } = useSelector(layoutSlice.stateSelector)
  const snackbar = useSelector(snackbarSlice.stateSelector)
  const [mode, setMode] = React.useState<'light' | 'dark'>('dark')
  useRefreshToken()
  useResetTokens()
  useGetBlogsAndSubs()
  useResetLoaderSlice()

  const classes = useStyles()

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

  return (
    <>
      <ThemeProvider theme={theme}>
        <div className={classes.root}>
          <CssBaseline />

          <Switch>
            <Route exact path={['/', '/:groupId']} component={AppNavbar} />
            <Route>
              <AppNavbar />
            </Route>
          </Switch>

          <main className={classes.content}>
            <div className={classes.toolbar} />
            <Container maxWidth="md">
              <Switch>
                <Route path="/login" component={Login} />
                <Route path="/register" component={Register} />
                <Route path="/blog/:blogid" exact component={SingleBlog} />
                <Route
                  path="/blog/:blogid/article/:articleid"
                  component={SingleArticle}
                />
                <Route exact path={['/', '/:groupId']} component={AppHome} />
                <Route>404</Route>
              </Switch>
            </Container>
          </main>
        </div>
        {loader != layoutSlice.type.none && <AppSpinner />}
        {snackbar.open && <CustomizedSnackbar />}
        <Footer />
      </ThemeProvider>
    </>
  )
}

export const drawerWidth = 290
const useStyles = makeStyles(theme => ({
  root: {
    display: 'flex',
  },
  drawer: {
    [theme.breakpoints.up('sm')]: {
      width: drawerWidth,
      flexShrink: 0,
    },
  },
  appBar: {
    [theme.breakpoints.up('sm')]: {
      width: `calc(100% - ${drawerWidth}px)`,
      marginLeft: drawerWidth,
    },
  },
  menuButton: {
    marginRight: theme.spacing(2),
    [theme.breakpoints.up('sm')]: {
      display: 'none',
    },
  },
  // necessary for content to be below app bar
  toolbar: theme.mixins.toolbar,
  drawerPaper: {
    width: drawerWidth,
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
}))

export default App
