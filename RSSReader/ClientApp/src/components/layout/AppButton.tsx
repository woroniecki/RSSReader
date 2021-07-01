import { Button, ButtonProps } from '@material-ui/core'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import styled, { css } from 'styled-components'

export const Wrapper = styled.div<{ weigth?: number }>`
button {
}
  ${_ =>
    _.weigth &&
    css`
      weight: ${_.weigth};
    `}
  }
`
export interface AppButtonProps extends ButtonProps {
  weight?: number
}

export const AppButton: React.FC<AppButtonProps> = ({ weight, ...props }) => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <Wrapper weigth={weight}>
      <Button {...props} />
    </Wrapper>
  )
}

export default AppButton
