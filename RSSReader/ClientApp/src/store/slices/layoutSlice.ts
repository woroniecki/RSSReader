import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { RootState } from 'store/rootReducer'
const AUTH = 'auth'
interface LayoutState {
  loader?: boolean
}

const initialState: LayoutState = {
  loader: null,
}

const layoutSlice = createSlice({
  name: 'layout',
  initialState,
  reducers: {
    setLoader: (state, action: PayloadAction<boolean>) => {
      state.loader = action.payload
    },
  },
})

export const { actions } = layoutSlice

export default layoutSlice.reducer

export const stateSelector = (state: RootState) => state.layoutSlice
