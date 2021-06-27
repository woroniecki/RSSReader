import React from 'react'
import clsx from 'clsx'
import { makeStyles, useTheme } from '@material-ui/core/styles'
import Drawer from '@material-ui/core/Drawer'
import CssBaseline from '@material-ui/core/CssBaseline'
import AppBar from '@material-ui/core/AppBar'
import Toolbar from '@material-ui/core/Toolbar'
import Typography from '@material-ui/core/Typography'
import Divider from '@material-ui/core/Divider'
import IconButton from '@material-ui/core/IconButton'
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft'
import ChevronRightIcon from '@material-ui/icons/ChevronRight'
import { useAppDispatch } from 'store/store'

import { useHistory, useParams } from 'react-router-dom'
import { layoutSlice, navbarSlice } from 'store/slices'
import UserNavbar from './UserNavbar'
import { useSelector } from 'react-redux'

export interface AppNavbarProps {}

const selectAuth = (state: { auth: any }) => state.auth

export const AppNavbar: React.FC<AppNavbarProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { loader } = useSelector(layoutSlice.stateSelector)
  const { groupId } = useParams<{ groupId: string }>()

  const classes = useStyles()
  const theme = useTheme()
  const { navOpen } = useSelector(navbarSlice.stateSelector)

  const handleDrawerOpen = () => {
    dispatch(navbarSlice.actions.setOpen(true))
  }

  const handleDrawerClose = () => {
    dispatch(navbarSlice.actions.setOpen(false))
  }

  const renderNavbar = () => {
    if (loader != layoutSlice.type.fullScreen) {
      return (
        <>
          <CssBaseline />
          <Drawer
            className={classes.drawer}
            variant="persistent"
            anchor="left"
            open={navOpen}
            classes={{
              paper: classes.drawerPaper,
            }}
          >
            <div className={classes.drawerHeader}>
              <IconButton onClick={handleDrawerClose}>
                {theme.direction === 'ltr' ? (
                  <ChevronLeftIcon />
                ) : (
                  <ChevronRightIcon />
                )}
              </IconButton>
            </div>
            <Divider />
            <UserNavbar curGroupId={parseInt(groupId)} />
          </Drawer>

          <AppBar
            position="fixed"
            className={clsx(classes.appBar, {
              [classes.appBarShift]: navOpen,
            })}
          >
            <Toolbar>
              <IconButton
                color="inherit"
                aria-label="open drawer"
                onClick={handleDrawerOpen}
                edge="start"
                className={clsx(classes.menuButton, navOpen && classes.hide)}
              >
                <ChevronRightIcon />
              </IconButton>
              <Typography variant="h6" noWrap>
                RSS Reader
              </Typography>
            </Toolbar>
          </AppBar>
        </>
      )
    } else {
      return <React.Fragment></React.Fragment>
    }
  }

  return renderNavbar()
}

const drawerWidth = 300

const useStyles = makeStyles(theme => ({
  appBar: {
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
  },
  appBarShift: {
    width: `calc(100% - ${drawerWidth}px)`,
    marginLeft: drawerWidth,
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  menuButton: {
    marginRight: theme.spacing(2),
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
}))

export default AppNavbar
