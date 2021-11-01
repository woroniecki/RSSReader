import { Button, IconButton, Typography } from '@material-ui/core'
import React, { MouseEventHandler } from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import PostAddIcon from '@material-ui/icons/PostAdd'

export interface NavbarActionProps {
  label: string
  children?: React.ReactNode
  onClick?: (event: React.MouseEvent<HTMLButtonElement>) => void
}

export const NavbarAction: React.FC<NavbarActionProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <Button onClick={props.onClick}>
      {props.children}
      <div>
        <Typography
          style={{ color: '#DFDFDF', fontSize: 10 }}
          variant="button"
          color="textSecondary"
        >
          {props.label}
        </Typography>
      </div>
    </Button>
  )
}

export default NavbarAction
