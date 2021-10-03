import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles'
import { GET_NODE_ENV, API_URL } from 'utils/envs'

export interface FooterProps {}

export const Footer: React.FC<FooterProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const classes = useStyles()

  return (
    <div className={classes.footer}>
      env: {GET_NODE_ENV()}, url: {API_URL()}
    </div>
  )
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    footer: {
      margin: 2,
      position: 'fixed',
      bottom: 6,
      right: 6,
    },
  })
)

export default Footer
