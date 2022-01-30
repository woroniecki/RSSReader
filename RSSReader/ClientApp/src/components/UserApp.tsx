import { Container } from '@material-ui/core'
import CssBaseline from '@material-ui/core/CssBaseline'
import {
  createMuiTheme,
  makeStyles,
  ThemeProvider,
} from '@material-ui/core/styles'
import AppNavbar from 'components/AppNavbar/AppNavbar'
import SingleArticle from 'components/Article/SingleArticle'
import Login from 'components/Auth/Login'
import Register from 'components/Auth/Register'
import BlogsList from 'components/Blog/BlogsList'
import SingleBlog from 'components/Blog/SingleBlog'
import React from 'react'
import { Route, Switch } from 'react-router'
import {} from 'styled-components'

interface Props {
  /**
   * Injected by the documentation to work in an iframe.
   * You won't need it on your project.
   */
  window?: () => Window
}

function UserApp(props: Props) {
  const [mode, setMode] = React.useState<'light' | 'dark'>('dark')

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
                <Route path={['/blog/:blogid', '/:groupId/blog/:blogid']} exact component={SingleBlog} />
                <Route
                  path={['/blog/:blogid/article/:articleid', '/:groupId/blog/:blogid/article/:articleid']}
                  component={SingleArticle}
                />
                <Route exact path={['/', '/:groupId']} component={BlogsList} />
                <Route>404</Route>
              </Switch>
            </Container>
          </main>
        </div>
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
    //padding: theme.spacing(3),
  },
}))

export default UserApp
