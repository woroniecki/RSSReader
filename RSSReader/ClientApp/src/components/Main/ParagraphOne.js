// @material-ui/core components
import { Box, Paper } from '@material-ui/core'
import { makeStyles } from '@material-ui/core/styles'
// @material-ui/icons
import Dashboard from '@material-ui/icons/Dashboard'
import List from '@material-ui/icons/List'
import Schedule from '@material-ui/icons/Schedule'
import appLookImg from 'components/Main/assets/img/applook.jpg'
import styles from 'components/Main/assets/jss/material-kit-react/views/componentsSections/pillsStyle.js'
// core components
import GridContainer from 'components/Main/components/Grid/GridContainer.js'
import GridItem from 'components/Main/components/Grid/GridItem.js'
import NavPills from 'components/Main/components/NavPills/NavPills.js'
import React from 'react'

const useStyles = makeStyles(styles)

export default function ParagraphOne() {
  const classes = useStyles()

  return (
    <div className={classes.section}>
      <div className={classes.container}>
        <div id="navigation-pills">
          <GridContainer>
            <GridItem xs={11} sm={11} md={8} lg={8}>
              <Box
                style={{
                  width: 75,
                  height: 15,
                  backgroundColor: '#9c27b0',
                }}
              />
              <div>
                <h1>A great reading experience</h1>
                <h3>
                  RSS Box has a clean interface with customizable themes and
                  typography for the optimal reading experience.
                </h3>
              </div>
            </GridItem>
          </GridContainer>
          <GridContainer>
            <GridItem xs={11} sm={11} md={8} lg={9}>
              <img
                src={appLookImg}
                alt="..."
                className={
                  classes.imgRaised +
                  ' ' +
                  classes.imgRounded +
                  ' ' +
                  classes.imgFluid
                }
              />
            </GridItem>
            <GridItem xs={11} sm={11} md={8} lg={3}>
              <h2>Fullscreen</h2>
              <h4>
                Reading is something that demands your full attention. Use the
                immersive full screen mode to bring the content you care about
                front and center.
              </h4>
              <h2>Fullscreen</h2>
              <h4>
                Reading is something that demands your full attention. Use the
                immersive full screen mode to bring the content you care about
                front and center.
              </h4>
            </GridItem>
          </GridContainer>
        </div>
      </div>
    </div>
  )
}
