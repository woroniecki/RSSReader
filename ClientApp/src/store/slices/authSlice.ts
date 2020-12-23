import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit'
import * as authApi from '../../api/authApi'
import { LoginRequest, LoginResponse } from '../../api/api.types'
import { RootState } from 'store/rootReducer'
const AUTH = 'auth'
interface AuthState {
  token?: string
  expiration?: number
  userName?: string
}

const initialState: AuthState = {
  expiration: 0,
  token: '',
  userName: '',
}

export const login = createAsyncThunk<
  // Return type of the payload creator
  LoginResponse,
  // First argument to the payload creator
  LoginRequest
>(`${AUTH}/login`, async (params, thunkApi) => {
  try {
    const res = await authApi.login(params)

    return res
  } catch (err) {
    throw err
  }
})

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    // login: (state, action: PayloadAction<number>) => state + action.payload,
    // logout: (state, action: PayloadAction<number>) => state - action.payload,
  },
  extraReducers: builder => {
    builder.addCase(login.fulfilled, (state, { payload }) => {
      state.token = payload.token
      state.expiration = payload.expiration
      state.userName = payload.user.userName
      // both `state` and `action` are now correctly typed
      // based on the slice state and the `pending` action creator
    })
  },
})

export const { actions } = authSlice

export default authSlice.reducer

export const stateSelector = (state: RootState) => state.authReducer
