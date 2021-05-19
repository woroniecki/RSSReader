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

export const postAddSubscription = createAsyncThunk<
  // Return type of the payload creator
  Subscription,
  // First argument to the payload creator
  AddSubscriptionRequest,
  {
    rejectValue: string
  }
>(`${SUBSCRIPTIONS}/addSubscription`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.postAddSubscribtions(params)
    return res
  } catch (err) {
    throw err.data
  }
})

export const putUnsubscribeBlog = createAsyncThunk<
  // Return type of the payload creator
  Subscription,
  // First argument to the payload creator
  number,
  {
    rejectValue: string
  }
>(`${SUBSCRIPTIONS}/unsubscribeBlog`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.putUnsubscribeBlog(params)
    return res
  } catch (err) {
    throw err.data
  }
})

export const patchGroup = createAsyncThunk<
  // Return type of the payload creator
  Subscription,
  // First argument to the payload creator
  PatchSubGroupRequest,
  {
    rejectValue: string
  }
>(`${SUBSCRIPTIONS}/patchGroup`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.patchSubscriptionGroup(
      params.subId,
      params.groupId
    )
    return res
  } catch (err) {
    throw err.data
  }
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
  },
  extraReducers: builder => {
    builder
      .addCase(getList.fulfilled, (state, { payload }) => {
        subscriptionsAdapter.setAll(state, payload)
        // state.entities[payload.id] = payload
        // both `state` and `action` are now correctly typed
        // based on the slice state and the `pending` action creator
      })
      .addCase(postAddSubscription.fulfilled, (state, { payload }) => {
        subscriptionsAdapter.addOne(state, payload)
      })
      .addCase(putUnsubscribeBlog.fulfilled, (state, { payload }) => {
        subscriptionsAdapter.removeOne(state, payload.id)
      })
      .addCase(patchGroup.fulfilled, (state, { payload }) => {
        state.entities[payload.id].groupId = payload.groupId
      })
      .addCase(patchSubscription.fulfilled, (state, { payload }) => {
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
