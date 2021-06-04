import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { Spinner } from 'react-bootstrap'

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

  return (
    <Spinner style={getSize()} animation="border" variant={props.variant} />
  )
}

export default SpinnerElement
