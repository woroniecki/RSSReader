import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
  PayloadAction,
} from '@reduxjs/toolkit'
import * as blogApi from '../../api/blogApi'
import {
  Subscription,
  AddSubscriptionRequest,
  PatchSubGroupRequest,
  PatchSubscriptionRequest,
} from '../../api/api.types'
import { RootState } from 'store/rootReducer'
const SUBSCRIPTIONS = 'subscriptions'

export const subscriptionsAdapter = createEntityAdapter<Subscription>({
  selectId: sub => sub.id,
})

export const patchSubscription = createAsyncThunk<
  Subscription,
  PatchSubscriptionRequest,
  {
    rejectValue: string
  }
>(`${SUBSCRIPTIONS}/patchSubscription`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.patchSubscription(params)
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

const subscriptionsSlice = createSlice({
  name: 'subscriptions',
  initialState: subscriptionsAdapter.getInitialState(),
  reducers: {
    clear: state => {
      subscriptionsAdapter.removeAll(state)
    },
    resetGroup: (state, action: PayloadAction<number>) => {
      state.entities[action.payload].groupId = -1
    },
    remove: (state, action: PayloadAction<number>) => {
      subscriptionsAdapter.removeOne(state, action.payload)
    },
  },
  extraReducers: builder => {
    builder.addCase(patchSubscription.fulfilled, (state, { payload }) => {
      state.entities[payload.id].filterReaded = payload.filterReaded
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
