import { ListItemIcon } from '@material-ui/core'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface AppListItemIconProps {
  style?: React.CSSProperties
  children?: React.ReactNode
}

export const AppListItemIcon: React.FC<AppListItemIconProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <ListItemIcon style={{ ...props.style, minWidth: '27px' }}>
      {props.children}
    </ListItemIcon>
  )
}

export default AppListItemIcon
