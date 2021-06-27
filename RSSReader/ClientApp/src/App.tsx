import logo from './logo.svg'
import './App.css'
import React from 'react'
import clsx from 'clsx'
import {} from 'styled-components'
import CssBaseline from '@material-ui/core/CssBaseline'
import { makeStyles } from '@material-ui/core/styles'
import AppHome from 'components/AppHome'
import AppNavbar from 'components/AppNavbar/AppNavbar'
import Login from 'components/Auth/Login'
import Register from 'components/Auth/Register'
import { Router, Switch, Route } from 'react-router'
import AppSpinner from './components/Spinner/AppSpinner'
import { useSelector } from 'react-redux'
import { layoutSlice } from 'store/slices'
import useRefreshToken from 'components/Auth/useRefreshToken'
import useResetTokens from 'components/Auth/useResetTokens'
import useGetBlogsAndSubs from 'components/Blog/useGetBlogsAndSubs'
import useResetLoaderSlice from 'components/Spinner/useResetLoaderSlice'
import SingleBlog from 'components/Blog/SingleBlog'
import SingleArticle from 'components/Article/SingleArticle'
import { useEffect } from 'react'
import { useAppDispatch } from 'store/store'
import { useHistory } from 'react-router-dom'

function App() {
  const { loader } = useSelector(layoutSlice.stateSelector)
  useRefreshToken()
  useResetTokens()
  useGetBlogsAndSubs()
  useResetLoaderSlice()

  const classes = useStyles()

  return (
    <>
      <div className={classes.root}>
        <CssBaseline />

        <Switch>
          <Route exact path={['/', '/:groupId']} component={AppNavbar} />
          <Route>
            <AppNavbar />
          </Route>
        </Switch>

        <main
          className={clsx(classes.content, {
            [classes.contentShift]: open,
          })}
        >
          <div className={classes.drawerHeader} />
          <Switch>
            <Route path="/login" component={Login} />
            <Route path="/register" component={Register} />
            <Route path="/blog/:id" exact component={SingleBlog} />
            <Route
              path="/blog/:id/article/:articleid"
              component={SingleArticle}
            />
            <Route exact path={['/', '/:groupId']} component={AppHome} />
            <Route>404</Route>
          </Switch>
        </main>
      </div>
      {loader != layoutSlice.type.none && <AppSpinner />}
    </>
  )
}

const drawerWidth = 240
const useStyles = makeStyles(theme => ({
  root: {
    display: 'flex',
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    marginLeft: -drawerWidth,
  },
  contentShift: {
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
    marginLeft: 0,
  },
}))

export default App
