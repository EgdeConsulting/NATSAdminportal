import { useEffect, useState } from "react";
import {
  PaginatedTable,
  MsgViewButton,
  LoadingSpinner,
  SelectColumnFilter,
  MsgProps,
} from "components";
import { Row } from "react-table";

function MsgTable() {
  /**
   * This constant defines the configuration of the paginated table.
   * The “accessor” property is equal to a property contained in a data object.
   * More information at: https://tanstack.com/table/v8/docs/guide/column-defs
   */
  const columns = [
    {
      Header: "MsgTable",
      columns: [
        {
          Header: "Sequence Number",
          accessor: "sequenceNumber",
          disableFilters: true,
        },
        {
          Header: "Timestamp",
          accessor: "timestamp",
          disableFilters: true,
        },
        {
          Header: "Stream",
          accessor: "stream",
          Filter: SelectColumnFilter,
          filter: "includes",
        },
        {
          Header: "Subject",
          accessor: "subject",
          Filter: SelectColumnFilter,
          filter: "includes",
        },
        {
          Header: "Data",
          accessor: "data",
          disableFilters: true,
          Cell: (props: { row: Row }) => {
            return <MsgViewButton content={props.row.values} />;
          },
        },
      ],
    },
  ];

  const [allMessages, setAllMessages] = useState<MsgProps[]>([]);
  const [isIntervalRunning, setIsIntervalRunning] = useState(false);
  const [loading, setLoading] = useState(true);

  function getAllMessages() {
    fetch("/api/allMessages").then((res) => {
      if (res.ok) {
        res.json().then((data: MsgProps[]) => {
          setAllMessages(data);
          setLoading(false);
        });
      } else if (res.status == 429) {
        /**
         * Occasionally an API-request fails due to the backend taking too long
         * handshaking with the NATS-server. This is not a problem as the next
         * API-request finished just fine, and new data is requested at a frequent rate.
         */
        console.log("API was too busy to handle request.");
      } else {
        alert(
          "An error occurred while fetching all messages: " + res.statusText
        );
      }
    });
  }

  /**
   * Starting an interval which frequently send new API-requests to get
   * the newest data. Only one interval is being started via the use of
   * the “isIntervalRunning” state. The interval is also being discarded
   * as soon as the component dismounts.
   */
  useEffect(() => {
    setIsIntervalRunning(true);
    const interval = setInterval(getAllMessages, 3000);
    return () => {
      clearInterval(interval);
      setIsIntervalRunning(false);
    };
  }, [!isIntervalRunning]);

  return (
    <>
      {loading ? (
        <LoadingSpinner />
      ) : (
        <PaginatedTable columns={columns} data={allMessages} />
      )}
    </>
  );
}

export { MsgTable };
