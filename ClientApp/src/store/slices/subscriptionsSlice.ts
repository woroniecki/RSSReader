import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
  PayloadAction,
} from '@reduxjs/toolkit'
import * as blogApi from '../../api/blogApi'
import {
  Subscription,
  Blog,
  SubscriptionsListResponse,
} from '../../api/api.types'
import { RootState } from 'store/rootReducer'
const SUBSCRIPTIONS = 'subscriptions'

export const subscriptionsAdapter = createEntityAdapter<Subscription>({
  selectId: sub => sub.id,
})

export const getList = createAsyncThunk<
  // Return type of the payload creator
  Subscription[],
  // First argument to the payload creator
  void,
  {
    rejectValue: string
  }
>(`${SUBSCRIPTIONS}/list`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.getSubscribtionsList()
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

const subscriptionsSlice = createSlice({
  name: 'subscriptions',
  initialState: subscriptionsAdapter.getInitialState(),
  reducers: {},
  extraReducers: builder => {
    builder.addCase(getList.fulfilled, (state, { payload }) => {
      subscriptionsAdapter.setAll(state, payload)
      //state.entities[payload.id] = payload
      // both `state` and `action` are now correctly typed
      // based on the slice state and the `pending` action creator
    })
  },
})

export const { actions } = subscriptionsSlice

export default subscriptionsSlice.reducer

export const stateSelector = (state: RootState) => state.subscriptionsReducer

export const {
  selectAll,
  selectById,
  selectIds,
} = subscriptionsAdapter.getSelectors<RootState>(
  state => state.subscriptionsReducer
)
