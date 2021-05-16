import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit'
import * as authApi from '../../api/authApi'
import {
  LoginRequest,
  LoginResponse,
  RefreshRequest,
} from '../../api/api.types'
import { RootState } from 'store/rootReducer'
import { setAuthHeader } from 'utils/setHeader'
import {
  cleatTokenDataFromStorage as clearTokenDataFromStorage,
  saveTokenDataToStorage,
} from 'utils/appLocalStorage'
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
  LoginRequest,
  {
    rejectValue: string
  }
>(`${AUTH}/login`, async (params, { rejectWithValue }) => {
  try {
    const res = await authApi.login(params)
    return res
  } catch (err) {
    return rejectWithValue(err.data)
  }
})

export const refresh = createAsyncThunk<
  LoginResponse,
  RefreshRequest,
  {
    rejectValue: string
  }
>(`${AUTH}/refresh`, async (params, { rejectWithValue }) => {
  try {
    const res = await authApi.refresh(params)
    return res
  } catch (err) {
    return rejectWithValue(err.data)
  }
})

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    logout: state => {
      state.token = null
      state.expiration = null
      state.userName = null
      setAuthHeader(null)
      clearTokenDataFromStorage()
    },
    // logout: (state, action: PayloadAction<number>) => state - action.payload,
  },
  extraReducers: builder => {
    builder
      .addCase(login.fulfilled, (state, { payload }) => {
        state.token = payload.authToken.token
        state.expiration = payload.authToken.expires
        state.userName = payload.user.userName
        setAuthHeader(payload.authToken.token)
        saveTokenDataToStorage(
          payload.authToken.token,
          payload.authToken.expires,
          payload.refreshToken.token,
          payload.refreshToken.expires
        )
        // both `state` and `action` are now correctly typed
        // based on the slice state and the `pending` action creator
      })
      .addCase(refresh.fulfilled, (state, { payload }) => {
        state.token = payload.authToken.token
        state.expiration = payload.authToken.expires
        state.userName = payload.user.userName
        setAuthHeader(payload.authToken.token)
        saveTokenDataToStorage(
          payload.authToken.token,
          payload.authToken.expires,
          payload.refreshToken.token,
          payload.refreshToken.expires
        )
      })
  },
})

export const { actions } = authSlice

export default authSlice.reducer

export const stateSelector = (state: RootState) => state.authReducer