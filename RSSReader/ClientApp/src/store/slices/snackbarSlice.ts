import { Color } from '@material-ui/lab'
import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { RootState } from 'store/rootReducer'

interface SnackbarState {
  open: boolean
  color: Color
  msg: string
}

const initialState: SnackbarState = {
  open: false,
  color: 'success',
  msg: '',
}

const snackbarSlice = createSlice({
  name: 'snackbar',
  initialState,
  reducers: {
    setSnackbar: (state, action: PayloadAction<SnackbarState>) => {
      state.open = action.payload.open
      state.color = action.payload.color
      state.msg = action.payload.msg
    },
    setOpen: (state, action: PayloadAction<boolean>) => {
      state.open = action.payload
    },
  },
})

export const { actions } = snackbarSlice

export default snackbarSlice.reducer

export const stateSelector = (state: RootState): SnackbarState =>
  state.snackbarSlice
