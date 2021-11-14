import { Hidden } from '@material-ui/core'
import AppBar from '@material-ui/core/AppBar'
import Divider from '@material-ui/core/Divider'
import Drawer from '@material-ui/core/Drawer'
import IconButton from '@material-ui/core/IconButton'
import { makeStyles, useTheme } from '@material-ui/core/styles'
import Toolbar from '@material-ui/core/Toolbar'
import Typography from '@material-ui/core/Typography'
import MenuIcon from '@material-ui/icons/Menu'
import React from 'react'
import { useSelector } from 'react-redux'
import { layoutSlice, navbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import { drawerWidth } from '../UserApp'
import ActionsBar from './NavbarActions/ActionsBar'
import UserNavbar from './UserNavbar'

export interface AppNavbarProps {}

export const AppNavbar: React.FC<AppNavbarProps> = () => {
  const dispatch = useAppDispatch()
  const { loader } = useSelector(layoutSlice.stateSelector)

  const classes = useStyles()
  const theme = useTheme()
  const { navOpen } = useSelector(navbarSlice.stateSelector)

  const handleDrawerToggle = () => {
    dispatch(navbarSlice.actions.setOpen(!navOpen))
  }

  const drawer = (
    <div>
      <div className={classes.toolbar} />
      <Divider />
      <UserNavbar />
      <ActionsBar />
    </div>
  )

  const renderNavbar = () => {
    if (loader != layoutSlice.type.fullScreen) {
      return (
        <>
          <AppBar position="fixed" className={classes.appBar}>
            <Toolbar>
              <IconButton
                color="inherit"
                aria-label="open drawer"
                edge="start"
                onClick={handleDrawerToggle}
                className={classes.menuButton}
              >
                <MenuIcon />
              </IconButton>
              <Typography variant="h6" noWrap>
                Rss Box
              </Typography>
            </Toolbar>
          </AppBar>
          <nav className={classes.drawer} aria-label="mailbox folders">
            {/* The implementation can be swapped with js to avoid SEO duplication of links. */}
            <Hidden smUp implementation="css">
              <Drawer
                variant="temporary"
                anchor={theme.direction === 'rtl' ? 'right' : 'left'}
                open={navOpen}
                onClose={handleDrawerToggle}
                classes={{
                  paper: classes.drawerPaper,
                }}
                ModalProps={{
                  keepMounted: true, // Better open performance on mobile.
                }}
              >
                {drawer}
              </Drawer>
            </Hidden>
            <Hidden xsDown implementation="css">
              <Drawer
                classes={{
                  paper: classes.drawerPaper,
                }}
                variant="permanent"
                open
              >
                {drawer}
              </Drawer>
            </Hidden>
          </nav>
        </>
      )
    } else {
      return <React.Fragment></React.Fragment>
    }
  }

  return renderNavbar()
}

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
    background: '#262626',
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

export default AppNavbar
