// @material-ui/core components
import { Box, Paper } from '@material-ui/core'
import { makeStyles } from '@material-ui/core/styles'
// @material-ui/icons
import Dashboard from '@material-ui/icons/Dashboard'
import List from '@material-ui/icons/List'
import Schedule from '@material-ui/icons/Schedule'
import computerImg from 'components/Main/assets/img/computer.png'
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
    <div className={classes.sectiongrey}>
      <div className={classes.containerright}>
        <div id="navigation-pills">
          <GridContainer>
            <GridItem xs={11} sm={11} md={8} lg={12}>
              <Box
                style={{
                  width: 75,
                  height: 15,
                  backgroundColor: '#9c27b0',
                  marginLeft: 'auto',
                  marginRight: 0,
                }}
              />
              <div>
                <h1>Organize & Manage</h1>
                <h3>
                  Stay organized and find that really great article you read
                  last month.
                </h3>
              </div>
            </GridItem>
          </GridContainer>
        </div>
      </div>
      <div className={classes.container}>
        <div id="navigation-pills">
          <GridContainer>
            <GridItem xs={11} sm={11} md={8} lg={4}>
              <h2>Search</h2>
              <h4>
                RSS Box supports a powerful and expressive search syntax to find
                exactly what you&apos;re looking for.
              </h4>
              <h2>Organiaze</h2>
              <h4>
                Group your feeds by a simple to use and clear organized system.
              </h4>
              <h2>All devices</h2>
              <h4>
                Use the platform on any type of device which you want. It&apos;s
                fully responsive and looks great on all resolutions.
              </h4>
            </GridItem>
            <GridItem xs={11} sm={11} md={8} lg={8}>
              <img
                src={computerImg}
                alt="..."
                className={classes.imgRounded + ' ' + classes.imgFluid}
              />
            </GridItem>
          </GridContainer>
        </div>
      </div>
    </div>
  )
}
