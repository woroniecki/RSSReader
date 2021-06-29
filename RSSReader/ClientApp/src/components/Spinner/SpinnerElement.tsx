import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { CircularProgress } from '@material-ui/core'

export interface SpinnerElementProps {
  size: number
  variant:
    | 'primary'
    | 'secondary'
    | 'success'
    | 'danger'
    | 'warning'
    | 'info'
    | 'dark'
    | 'light'
    | string
}

export const SpinnerElement: React.FC<SpinnerElementProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  function getSize() {
    return { width: `${props.size}px`, height: `${props.size}px` }
  }

  return <CircularProgress size={props.size} />
}

export default SpinnerElement
