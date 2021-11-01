import { ListItemText, Typography } from '@material-ui/core'
import { Variant as ThemeVariant } from '@material-ui/core/styles/createTypography'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface AppListItemTextProps {
  fontSize: number
  variant: ThemeVariant
  text: string
}

export const AppListItemText: React.FC<AppListItemTextProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <ListItemText
      primary={
        <Typography
          variant={props.variant}
          style={{ color: '#DFDFDF', fontSize: props.fontSize }}
        >
          {props.text}
        </Typography>
      }
    />
  )
}

export default AppListItemText
